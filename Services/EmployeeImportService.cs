using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Utilities;

namespace SparkHRMS.Services
{
    /// <summary>
    /// Handles bulk employee creation from an uploaded Excel workbook:
    /// template generation, parsing/validation, duplicate detection, background
    /// insertion with live progress, an error workbook for the failed rows, and
    /// an in-memory log of past imports.
    /// </summary>
    public interface IEmployeeImportService
    {
        byte[] GenerateTemplate();

        /// <summary>
        /// Parse + validate the uploaded workbook header, then start background
        /// processing. Returns an importId for progress polling, or an error when
        /// the file itself is unusable (wrong format, missing columns, no rows).
        /// </summary>
        (bool ok, string importId, int total, string? error) Begin(byte[] fileBytes, string fileName, string? userName);

        ImportProgressDto? GetProgress(string importId);
        byte[]? GetErrorFile(string importId, out string? fileName);
    }

    // ---- Column definitions: the single source of truth for the template,
    // the parser and the error workbook. Header names are matched against the
    // uploaded file case-insensitively (ignoring spaces/punctuation). ----
    public sealed class ImportColumn
    {
        public string Key { get; init; } = "";
        public string Header { get; init; } = "";
        public bool Required { get; init; }
        public string Notes { get; init; } = "";
    }

    public sealed class ImportProgressDto
    {
        public string ImportId { get; set; } = "";
        public string FileName { get; set; } = "";
        public int Total { get; set; }
        public int Processed { get; set; }
        public int Succeeded { get; set; }
        public int Duplicates { get; set; }
        public int Failed { get; set; }
        public bool Done { get; set; }
        public bool HasErrorFile { get; set; }
        public string? Message { get; set; }
        public int Percent => Total <= 0 ? (Done ? 100 : 0) : (int)Math.Floor(Processed * 100.0 / Total);
    }

    public sealed class ImportHistoryItem
    {
        public string ImportId { get; set; } = "";
        public string FileName { get; set; } = "";
        public DateTime ImportedOn { get; set; }
        public string ImportedBy { get; set; } = "";
        public int Total { get; set; }
        public int Succeeded { get; set; }
        public int Duplicates { get; set; }
        public int Failed { get; set; }
        public bool HasErrorFile { get; set; }
    }

    public class EmployeeImportService : IEmployeeImportService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmployeeImportService> _logger;

        // Live jobs keyed by importId (progress tracking, request-scoped via service calls).
        private readonly ConcurrentDictionary<string, ImportJob> _jobs = new();
        // Note: History is now stored in HttpContext.Session via the controller,
        // not in the service, so it clears per-session/per-page-load.

        private const int MinimumEmployeeAge = 18;
        private const string DefaultImageUrl = "/images/no-image.png";

        public EmployeeImportService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<EmployeeImportService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        // Order here is the column order used in the template and error workbook.
        public static readonly IReadOnlyList<ImportColumn> Columns = new List<ImportColumn>
        {
            new() { Key = "EmployeeCode",             Header = "EmployeeCode",             Notes = "Optional. Unique code if you use one." },
            new() { Key = "FullName",                 Header = "FullName",                 Required = true,  Notes = "Required." },
            new() { Key = "Gender",                   Header = "Gender",                   Notes = "Male / Female." },
            new() { Key = "DateOfBirth",              Header = "DateOfBirth",              Notes = "dd-MM-yyyy. Employee must be at least 18." },
            new() { Key = "BloodGroup",               Header = "BloodGroup",               Notes = "A+, A-, B+, B-, O+, O-, AB+, AB-." },
            new() { Key = "DateOfJoining",            Header = "DateOfJoining",            Notes = "dd-MM-yyyy." },
            new() { Key = "Nationality",              Header = "Nationality",              Notes = "Indian / Others." },
            new() { Key = "Designation",              Header = "Designation",              Notes = "" },
            new() { Key = "ReportingHead",            Header = "ReportingHead",            Notes = "Name - Designation of the reporting head." },
            new() { Key = "MaritalStatus",            Header = "MaritalStatus",            Notes = "Single / Married / Divorced / Widowed." },
            new() { Key = "PersonalMobileNumber",     Header = "PersonalMobileNumber",     Required = true,  Notes = "Required. 10 digits." },
            new() { Key = "AlternateMobileNumber",    Header = "AlternateMobileNumber",    Notes = "Must differ from the personal mobile." },
            new() { Key = "PersonalEmail",            Header = "PersonalEmail",            Required = true,  Notes = "Required. Used as the login id; must be unique." },
            new() { Key = "CurrentAddressLine1",      Header = "CurrentAddressLine1",      Required = true,  Notes = "Required." },
            new() { Key = "CurrentAddressLine2",      Header = "CurrentAddressLine2",      Notes = "" },
            new() { Key = "CurrentCity",              Header = "CurrentCity",              Required = true,  Notes = "Required." },
            new() { Key = "CurrentState",             Header = "CurrentState",             Notes = "Tamil Nadu / Kerala." },
            new() { Key = "CurrentCountry",           Header = "CurrentCountry",           Required = true,  Notes = "Required, e.g. India." },
            new() { Key = "CurrentPincode",           Header = "CurrentPincode",           Required = true,  Notes = "Required. 6 digits." },
            new() { Key = "PermanentAddressLine1",    Header = "PermanentAddressLine1",    Notes = "Leave the permanent columns blank to copy the current address." },
            new() { Key = "PermanentAddressLine2",    Header = "PermanentAddressLine2",    Notes = "" },
            new() { Key = "PermanentCity",            Header = "PermanentCity",            Notes = "" },
            new() { Key = "PermanentState",           Header = "PermanentState",           Notes = "Tamil Nadu / Kerala." },
            new() { Key = "PermanentCountry",         Header = "PermanentCountry",         Notes = "" },
            new() { Key = "PermanentPincode",         Header = "PermanentPincode",         Notes = "" },
            new() { Key = "EmergencyContactName",     Header = "EmergencyContactName",     Notes = "" },
            new() { Key = "EmergencyContactRelation", Header = "EmergencyContactRelation", Notes = "Father / Mother / Sibling / Spouse / Friend." },
            new() { Key = "EmergencyContactNumber",   Header = "EmergencyContactNumber",   Notes = "" },
            new() { Key = "EmergencyAlternateNumber", Header = "EmergencyAlternateNumber", Notes = "" },
            new() { Key = "FatherName",               Header = "FatherName",               Notes = "" },
            new() { Key = "MotherName",               Header = "MotherName",               Notes = "" },
            new() { Key = "SpouseName",               Header = "SpouseName",               Notes = "" },
            new() { Key = "NumberOfDependents",       Header = "NumberOfDependents",       Notes = "Whole number." },
            new() { Key = "PANNumber",                Header = "PANNumber",                Notes = "" },
            new() { Key = "AadhaarNumber",            Header = "AadhaarNumber",            Notes = "12 digits." },
            new() { Key = "PassportNumber",           Header = "PassportNumber",           Notes = "" },
            new() { Key = "PassportExpiryDate",       Header = "PassportExpiryDate",       Notes = "dd-MM-yyyy." },
            new() { Key = "DrivingLicenseNumber",     Header = "DrivingLicenseNumber",     Notes = "" },
        };

        private static readonly Dictionary<string, int> StateNameToId =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "Tamil Nadu", 1 },
                { "TamilNadu", 1 },
                { "Kerala", 2 },
            };

        // ---------------------------------------------------------------- Template

        public byte[] GenerateTemplate()
        {
            using var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("Employees");
            for (int i = 0; i < Columns.Count; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = Columns[i].Header;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = Columns[i].Required ? XLColor.FromArgb(0, 150, 136) : XLColor.FromArgb(224, 224, 224);
                cell.Style.Font.FontColor = Columns[i].Required ? XLColor.White : XLColor.Black;
            }
            ws.SheetView.FreezeRows(1);
            ws.Columns().AdjustToContents();

            // Instructions sheet so the header row stays clean for parsing.
            var ins = wb.Worksheets.Add("Instructions");
            ins.Cell(1, 1).Value = "Column";
            ins.Cell(1, 2).Value = "Required";
            ins.Cell(1, 3).Value = "Notes";
            ins.Range(1, 1, 1, 3).Style.Font.Bold = true;
            int r = 2;
            foreach (var c in Columns)
            {
                ins.Cell(r, 1).Value = c.Header;
                ins.Cell(r, 2).Value = c.Required ? "Yes" : "No";
                ins.Cell(r, 3).Value = c.Notes;
                r++;
            }
            ins.Cell(r + 1, 1).Value = "Tip: Do not rename or remove the header row on the 'Employees' sheet. Add one employee per row below the header.";
            ins.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        // ------------------------------------------------------------------ Begin

        public (bool ok, string importId, int total, string? error) Begin(byte[] fileBytes, string fileName, string? userName)
        {
            List<ParsedRow> rows;
            try
            {
                rows = ParseWorkbook(fileBytes, out var headerError);
                if (headerError != null)
                    return (false, "", 0, headerError);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read uploaded employee workbook {File}", fileName);
                return (false, "", 0, "The file could not be read. Please upload a valid .xlsx file created from the template.");
            }

            if (rows.Count == 0)
                return (false, "", 0, "No employee rows were found in the file.");

            var importId = Guid.NewGuid().ToString("N");
            var job = new ImportJob
            {
                Progress = new ImportProgressDto
                {
                    ImportId = importId,
                    FileName = fileName,
                    Total = rows.Count
                },
                UserName = userName ?? "System"
            };
            _jobs[importId] = job;

            // Process off the request thread so the client can poll progress.
            _ = Task.Run(() => ProcessAsync(job, rows));

            return (true, importId, rows.Count, null);
        }

        public ImportProgressDto? GetProgress(string importId)
        {
            if (!_jobs.TryGetValue(importId, out var job)) return null;
            lock (job.Sync)
            {
                // Return a snapshot so callers never see a half-updated object.
                return new ImportProgressDto
                {
                    ImportId = job.Progress.ImportId,
                    FileName = job.Progress.FileName,
                    Total = job.Progress.Total,
                    Processed = job.Progress.Processed,
                    Succeeded = job.Progress.Succeeded,
                    Duplicates = job.Progress.Duplicates,
                    Failed = job.Progress.Failed,
                    Done = job.Progress.Done,
                    HasErrorFile = job.Progress.HasErrorFile,
                    Message = job.Progress.Message
                };
            }
        }

        public byte[]? GetErrorFile(string importId, out string? fileName)
        {
            fileName = null;
            if (!_jobs.TryGetValue(importId, out var job)) return null;
            lock (job.Sync)
            {
                if (job.ErrorFile == null) return null;
                fileName = "ImportErrors_" + Path.GetFileNameWithoutExtension(job.Progress.FileName) + ".xlsx";
                return job.ErrorFile;
            }
        }


        // ------------------------------------------------------------- Processing

        private async Task ProcessAsync(ImportJob job, List<ParsedRow> rows)
        {
            // A fresh DI scope: the request scope that started the import is long gone.
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var defaultPassword = _configuration["AppSettings:DefaultUserPassword"];

            // Existing contacts for duplicate detection against the database.
            var existing = await context.Employees
                .Select(e => new { e.PhoneNumber, e.Email })
                .ToListAsync();
            var dbPhones = new HashSet<string>(existing.Select(e => NormalizeMobile(e.PhoneNumber)).Where(s => s.Length > 0));
            var dbEmails = new HashSet<string>(existing.Select(e => (e.Email ?? "").Trim().ToLowerInvariant()).Where(s => s.Length > 0));

            // Contacts seen earlier in THIS file (catches in-file duplicates).
            var seenPhones = new HashSet<string>();
            var seenEmails = new HashSet<string>();

            var failedRows = new List<(ParsedRow row, string reason)>();

            foreach (var row in rows)
            {
                string? reason = null;
                bool isDuplicate = false;

                try
                {
                    var validation = ValidateAndBuild(row, out var staged);
                    if (validation != null)
                    {
                        reason = validation;
                    }
                    else
                    {
                        var phoneKey = NormalizeMobile(staged.Phone);
                        var emailKey = staged.Email.Trim().ToLowerInvariant();

                        if ((phoneKey.Length > 0 && (dbPhones.Contains(phoneKey) || seenPhones.Contains(phoneKey))))
                        {
                            reason = "Duplicate: this mobile number already exists.";
                            isDuplicate = true;
                        }
                        else if (emailKey.Length > 0 && (dbEmails.Contains(emailKey) || seenEmails.Contains(emailKey)))
                        {
                            reason = "Duplicate: this email already exists.";
                            isDuplicate = true;
                        }
                        else
                        {
                            reason = await InsertAsync(context, userManager, staged, defaultPassword);
                            if (reason == null)
                            {
                                // Reserve these contacts so later rows in the same file conflict.
                                if (phoneKey.Length > 0) { seenPhones.Add(phoneKey); }
                                if (emailKey.Length > 0) { seenEmails.Add(emailKey); }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error importing row {Row}", row.RowNumber);
                    reason = "Unexpected error: " + (ex.InnerException?.Message ?? ex.Message);
                }

                lock (job.Sync)
                {
                    job.Progress.Processed++;
                    if (reason == null) job.Progress.Succeeded++;
                    else
                    {
                        job.Progress.Failed++;
                        if (isDuplicate) job.Progress.Duplicates++;
                        failedRows.Add((row, reason));
                    }
                }
            }

            byte[]? errorBytes = failedRows.Count > 0 ? BuildErrorWorkbook(failedRows) : null;

            lock (job.Sync)
            {
                job.ErrorFile = errorBytes;
                job.Progress.HasErrorFile = errorBytes != null;
                job.Progress.Done = true;
                job.Progress.Message = $"Imported {job.Progress.Succeeded} of {job.Progress.Total}. " +
                                       $"{job.Progress.Duplicates} duplicate(s), {job.Progress.Failed - job.Progress.Duplicates} error(s).";
            }
            // History is now added to session by the controller endpoint.
        }

        // Inserts one employee (login + employee + addresses) in its own
        // transaction. Returns null on success, or an error reason on failure.
        private async Task<string?> InsertAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            StagedEmployee s,
            string? defaultPassword)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var user = new ApplicationUser
                {
                    UserName = s.Email,
                    Email = s.Email,
                    PhoneNumber = s.Phone,
                    IsActive = true
                };

                var userResult = await userManager.CreateAsync(user, defaultPassword);
                if (!userResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return "Login account: " + string.Join("; ", userResult.Errors.Select(e => e.Description));
                }

                var roleResult = await userManager.AddToRoleAsync(user, Roles.RoleType.Employee.ToString());
                if (!roleResult.Succeeded)
                {
                    // Rolling back undoes the user insert too, so no manual delete needed.
                    await transaction.RollbackAsync();
                    return "Failed to assign Employee role.";
                }

                var employee = new Employee
                {
                    EmployeeCode = s.EmployeeCode,
                    Name = s.FullName,
                    Gender = s.Gender,
                    DOB = s.DOB ?? DateTime.Now,
                    BloodGroup = s.BloodGroup,
                    Nationality = s.Nationality,
                    MaritalStatus = s.MaritalStatus,
                    PhoneNumber = s.Phone,
                    Email = s.Email,
                    FatherName = s.FatherName,
                    MotherName = s.MotherName,
                    SpouseName = s.SpouseName,
                    AadhaarNumber = s.AadhaarNumber,
                    PANNumber = s.PanNumber,
                    PassportNumber = s.PassportNumber,
                    PassportExpiryDate = s.PassportExpiryDate,
                    NumberOfDependents = s.NumberOfDependents,
                    DrivingLicenseNumber = s.DrivingLicenseNumber,
                    Designation = s.Designation,
                    ImageUrl = DefaultImageUrl,
                    DateOfJoining = s.DateOfJoining,
                    ReportingHeadMailID = s.ReportingHead,
                    AlternateMobileNumber = s.AlternatePhone,
                    EmergencyContactName = s.EmergencyContactName,
                    EmergencyContactNumber = s.EmergencyContactNumber,
                    EmergencyContactRelation = s.EmergencyContactRelation,
                    EmergencyAlternateNumber = s.EmergencyAlternateNumber,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    ApplicationUserId = user.Id
                };

                context.Employees.Add(employee);
                await context.SaveChangesAsync();

                var current = new Address
                {
                    Address1 = s.CurrentAddress1,
                    Address2 = s.CurrentAddress2,
                    City = s.CurrentCity,
                    StateId = s.CurrentStateId,
                    Country = string.IsNullOrWhiteSpace(s.CurrentCountry) ? "India" : s.CurrentCountry,
                    Pincode = s.CurrentPincode,
                    DocumentType = "MST_Employee",
                    DocumentId = employee.EmployeeId,
                    IsPermanentAddress = false
                };
                context.MST_Address.Add(current);

                var permanent = new Address
                {
                    Address1 = s.PermanentAddress1,
                    Address2 = s.PermanentAddress2,
                    City = s.PermanentCity,
                    StateId = s.PermanentStateId,
                    Country = string.IsNullOrWhiteSpace(s.PermanentCountry) ? "India" : s.PermanentCountry,
                    Pincode = s.PermanentPincode,
                    DocumentType = "MST_Employee",
                    DocumentId = employee.EmployeeId,
                    IsPermanentAddress = true
                };
                context.MST_Address.Add(permanent);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ex.InnerException?.Message ?? ex.Message;
            }
            finally
            {
                // Clear per row so a rolled-back (failed) row leaves no stray
                // Added entities that the next row would re-insert, and so the
                // long-running loop doesn't accumulate tracked entities.
                context.ChangeTracker.Clear();
            }
        }

        // --------------------------------------------------------- Validation

        // Returns an error message, or null when the row is valid (and fills staged).
        private string? ValidateAndBuild(ParsedRow row, out StagedEmployee staged)
        {
            staged = new StagedEmployee();

            string Get(string key) => row.Values.TryGetValue(key, out var v) ? (v ?? "").Trim() : "";

            var fullName = Get("FullName");
            if (string.IsNullOrWhiteSpace(fullName))
                return "Full Name is required.";

            var phone = Get("PersonalMobileNumber");
            if (string.IsNullOrWhiteSpace(phone))
                return "Personal Mobile Number is required.";

            var email = Get("PersonalEmail");
            if (string.IsNullOrWhiteSpace(email))
                return "Personal Email is required.";
            if (!new EmailAddressAttribute().IsValid(email))
                return "Personal Email is not a valid email address.";

            var altPhone = Get("AlternateMobileNumber");
            if (AreMobileNumbersSame(phone, altPhone))
                return "Personal and Alternate mobile numbers cannot be the same.";

            // Date of Birth (optional, but if present must be 18+).
            DateTime? dob = null;
            var dobRaw = Get("DateOfBirth");
            if (!string.IsNullOrWhiteSpace(dobRaw))
            {
                dob = ParseDate(dobRaw);
                if (dob == null)
                    return $"Date of Birth '{dobRaw}' is not a valid date (use dd-MM-yyyy).";
                if (IsUnderMinimumAge(dob))
                    return $"Employee must be at least {MinimumEmployeeAge} years old (check Date of Birth).";
            }

            DateTime? doj = null;
            var dojRaw = Get("DateOfJoining");
            if (!string.IsNullOrWhiteSpace(dojRaw))
            {
                doj = ParseDate(dojRaw);
                if (doj == null)
                    return $"Date of Joining '{dojRaw}' is not a valid date (use dd-MM-yyyy).";
            }

            DateTime? passportExpiry = null;
            var passRaw = Get("PassportExpiryDate");
            if (!string.IsNullOrWhiteSpace(passRaw))
            {
                passportExpiry = ParseDate(passRaw);
                if (passportExpiry == null)
                    return $"Passport Expiry Date '{passRaw}' is not a valid date (use dd-MM-yyyy).";
            }

            // Current address (required block).
            var cAddr1 = Get("CurrentAddressLine1");
            var cCity = Get("CurrentCity");
            var cCountry = Get("CurrentCountry");
            var cPincode = Get("CurrentPincode");
            if (string.IsNullOrWhiteSpace(cAddr1)) return "Current Address Line 1 is required.";
            if (string.IsNullOrWhiteSpace(cCity)) return "Current City is required.";
            if (string.IsNullOrWhiteSpace(cCountry)) return "Current Country is required.";
            if (string.IsNullOrWhiteSpace(cPincode)) return "Current Pincode is required.";

            // Permanent address: if every permanent column is blank, copy current.
            var pAddr1 = Get("PermanentAddressLine1");
            var pAddr2 = Get("PermanentAddressLine2");
            var pCity = Get("PermanentCity");
            var pState = Get("PermanentState");
            var pCountry = Get("PermanentCountry");
            var pPincode = Get("PermanentPincode");

            bool permanentBlank = string.IsNullOrWhiteSpace(pAddr1) && string.IsNullOrWhiteSpace(pAddr2)
                && string.IsNullOrWhiteSpace(pCity) && string.IsNullOrWhiteSpace(pState)
                && string.IsNullOrWhiteSpace(pCountry) && string.IsNullOrWhiteSpace(pPincode);

            if (permanentBlank)
            {
                pAddr1 = cAddr1;
                pAddr2 = Get("CurrentAddressLine2");
                pCity = cCity;
                pState = Get("CurrentState");
                pCountry = cCountry;
                pPincode = cPincode;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(pAddr1)) return "Permanent Address Line 1 is required (or leave all permanent columns blank to copy current).";
                if (string.IsNullOrWhiteSpace(pCity)) return "Permanent City is required.";
                if (string.IsNullOrWhiteSpace(pCountry)) return "Permanent Country is required.";
                if (string.IsNullOrWhiteSpace(pPincode)) return "Permanent Pincode is required.";
            }

            int dependents = 0;
            var depRaw = Get("NumberOfDependents");
            if (!string.IsNullOrWhiteSpace(depRaw) && !int.TryParse(depRaw, out dependents))
                return $"Number of Dependents '{depRaw}' is not a whole number.";

            staged.EmployeeCode = NullIfEmpty(Get("EmployeeCode"));
            staged.FullName = fullName;
            staged.Gender = NullIfEmpty(Get("Gender"));
            staged.DOB = dob;
            staged.BloodGroup = NullIfEmpty(Get("BloodGroup"));
            staged.DateOfJoining = doj;
            staged.Nationality = NullIfEmpty(Get("Nationality"));
            staged.Designation = NullIfEmpty(Get("Designation"));
            staged.ReportingHead = NullIfEmpty(Get("ReportingHead"));
            staged.MaritalStatus = NullIfEmpty(Get("MaritalStatus"));
            staged.Phone = phone;
            staged.AlternatePhone = NullIfEmpty(altPhone);
            staged.Email = email;
            staged.CurrentAddress1 = cAddr1;
            staged.CurrentAddress2 = NullIfEmpty(Get("CurrentAddressLine2"));
            staged.CurrentCity = cCity;
            staged.CurrentStateId = ResolveState(Get("CurrentState"));
            staged.CurrentCountry = cCountry;
            staged.CurrentPincode = cPincode;
            staged.PermanentAddress1 = pAddr1;
            staged.PermanentAddress2 = NullIfEmpty(pAddr2);
            staged.PermanentCity = pCity;
            staged.PermanentStateId = ResolveState(pState);
            staged.PermanentCountry = pCountry;
            staged.PermanentPincode = pPincode;
            staged.EmergencyContactName = NullIfEmpty(Get("EmergencyContactName"));
            staged.EmergencyContactRelation = NullIfEmpty(Get("EmergencyContactRelation"));
            staged.EmergencyContactNumber = NullIfEmpty(Get("EmergencyContactNumber"));
            staged.EmergencyAlternateNumber = NullIfEmpty(Get("EmergencyAlternateNumber"));
            staged.FatherName = NullIfEmpty(Get("FatherName"));
            staged.MotherName = NullIfEmpty(Get("MotherName"));
            staged.SpouseName = NullIfEmpty(Get("SpouseName"));
            staged.NumberOfDependents = dependents;
            staged.PanNumber = NullIfEmpty(Get("PANNumber"));
            staged.AadhaarNumber = NullIfEmpty(Get("AadhaarNumber"));
            staged.PassportNumber = NullIfEmpty(Get("PassportNumber"));
            staged.PassportExpiryDate = passportExpiry;
            staged.DrivingLicenseNumber = NullIfEmpty(Get("DrivingLicenseNumber"));

            return null;
        }

        // ------------------------------------------------------------- Parsing

        private List<ParsedRow> ParseWorkbook(byte[] fileBytes, out string? headerError)
        {
            headerError = null;
            var result = new List<ParsedRow>();

            using var ms = new MemoryStream(fileBytes);
            using var wb = new XLWorkbook(ms);

            // Prefer the "Employees" sheet; fall back to the first worksheet.
            var ws = wb.Worksheets.FirstOrDefault(w => w.Name.Equals("Employees", StringComparison.OrdinalIgnoreCase))
                     ?? wb.Worksheets.FirstOrDefault();
            if (ws == null)
            {
                headerError = "The workbook has no worksheets.";
                return result;
            }

            var firstRow = ws.FirstRowUsed();
            if (firstRow == null)
            {
                headerError = "The sheet is empty. Please use the downloaded template.";
                return result;
            }

            // Map each known column Key to its physical column number by matching
            // the header text (normalized: letters/digits only, lower-cased).
            var headerByCol = new Dictionary<int, string>();
            foreach (var cell in firstRow.CellsUsed())
            {
                var norm = Normalize(cell.GetString());
                var match = Columns.FirstOrDefault(c => Normalize(c.Header) == norm);
                if (match != null && !headerByCol.ContainsValue(match.Key))
                    headerByCol[cell.Address.ColumnNumber] = match.Key;
            }

            var missingRequired = Columns
                .Where(c => c.Required && !headerByCol.ContainsValue(c.Key))
                .Select(c => c.Header)
                .ToList();
            if (missingRequired.Count > 0)
            {
                headerError = "The file is missing required column(s): " + string.Join(", ", missingRequired) +
                              ". Please download a fresh template.";
                return result;
            }

            int headerRowNumber = firstRow.RowNumber();
            foreach (var dataRow in ws.RowsUsed().Where(r => r.RowNumber() > headerRowNumber))
            {
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                bool anyValue = false;

                foreach (var kv in headerByCol)
                {
                    var cell = dataRow.Cell(kv.Key);
                    string text;
                    if (cell.DataType == XLDataType.DateTime && cell.TryGetValue(out DateTime dt))
                        text = dt.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                    else
                        text = cell.GetString().Trim();

                    if (!string.IsNullOrWhiteSpace(text)) anyValue = true;
                    values[kv.Value] = text;
                }

                if (!anyValue) continue; // skip fully-blank rows
                result.Add(new ParsedRow { RowNumber = dataRow.RowNumber(), Values = values });
            }

            return result;
        }

        private byte[] BuildErrorWorkbook(List<(ParsedRow row, string reason)> failedRows)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Errors");

            // First column = error reason, then every template column.
            ws.Cell(1, 1).Value = "ErrorReason";
            for (int i = 0; i < Columns.Count; i++)
                ws.Cell(1, i + 2).Value = Columns[i].Header;
            var header = ws.Range(1, 1, 1, Columns.Count + 1);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.FromArgb(244, 67, 54);
            header.Style.Font.FontColor = XLColor.White;
            ws.SheetView.FreezeRows(1);

            int r = 2;
            foreach (var (row, reason) in failedRows)
            {
                ws.Cell(r, 1).Value = reason;
                for (int i = 0; i < Columns.Count; i++)
                {
                    var v = row.Values.TryGetValue(Columns[i].Key, out var s) ? s : "";
                    ws.Cell(r, i + 2).Value = v;
                }
                r++;
            }
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        // ------------------------------------------------------------- Helpers

        private static string Normalize(string s) =>
            new string((s ?? "").Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

        private static string? NullIfEmpty(string s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static int ResolveState(string state)
        {
            if (string.IsNullOrWhiteSpace(state)) return 0;
            state = state.Trim();
            if (StateNameToId.TryGetValue(state, out var id)) return id;
            return int.TryParse(state, out var n) ? n : 0;
        }

        private static DateTime? ParseDate(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            raw = raw.Trim();
            string[] formats = { "dd-MM-yyyy", "d-M-yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "dd.MM.yyyy" };
            if (DateTime.TryParseExact(raw, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
            // Last resort: en-GB (day-first) general parse.
            if (DateTime.TryParse(raw, new CultureInfo("en-GB"), DateTimeStyles.None, out dt))
                return dt;
            return null;
        }

        private static string NormalizeMobile(string? number)
        {
            if (string.IsNullOrWhiteSpace(number)) return string.Empty;
            return new string(number.Where(char.IsDigit).ToArray()).TrimStart('0');
        }

        private static bool AreMobileNumbersSame(string? phone, string? altPhone)
        {
            var p = NormalizeMobile(phone);
            var a = NormalizeMobile(altPhone);
            return p.Length > 0 && p == a;
        }

        private static bool IsUnderMinimumAge(DateTime? dob)
        {
            if (!dob.HasValue) return false;
            var today = DateTime.Today;
            var age = today.Year - dob.Value.Year;
            if (dob.Value.Date > today.AddYears(-age)) age--;
            return age < MinimumEmployeeAge;
        }

        // --------------------------------------------------------- Internal types

        private sealed class ImportJob
        {
            public ImportProgressDto Progress { get; init; } = new();
            public byte[]? ErrorFile { get; set; }
            public string UserName { get; init; } = "System";
            public object Sync { get; } = new();
        }

        public sealed class ParsedRow
        {
            public int RowNumber { get; set; }
            public Dictionary<string, string> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        }

        private sealed class StagedEmployee
        {
            public string? EmployeeCode;
            public string FullName = "";
            public string? Gender;
            public DateTime? DOB;
            public string? BloodGroup;
            public DateTime? DateOfJoining;
            public string? Nationality;
            public string? Designation;
            public string? ReportingHead;
            public string? MaritalStatus;
            public string Phone = "";
            public string? AlternatePhone;
            public string Email = "";
            public string CurrentAddress1 = "";
            public string? CurrentAddress2;
            public string CurrentCity = "";
            public int CurrentStateId;
            public string CurrentCountry = "";
            public string CurrentPincode = "";
            public string PermanentAddress1 = "";
            public string? PermanentAddress2;
            public string PermanentCity = "";
            public int PermanentStateId;
            public string PermanentCountry = "";
            public string PermanentPincode = "";
            public string? EmergencyContactName;
            public string? EmergencyContactRelation;
            public string? EmergencyContactNumber;
            public string? EmergencyAlternateNumber;
            public string? FatherName;
            public string? MotherName;
            public string? SpouseName;
            public int NumberOfDependents;
            public string? PanNumber;
            public string? AadhaarNumber;
            public string? PassportNumber;
            public DateTime? PassportExpiryDate;
            public string? DrivingLicenseNumber;
        }
    }
}
