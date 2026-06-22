using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Utilities;
using SparkHRMS.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SparkHRMS.Controllers
{
    //[Authorize(Roles = "SuperAdmin,Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        // Minimum age (in years) an employee must be, based on Date of Birth.
        private const int MinimumEmployeeAge = 18;

        // True when a DOB is provided and the resulting age (as of today) is below
        // the minimum. A null DOB is not validated here (it is optional).
        private static bool IsUnderMinimumAge(DateTime? dob)
        {
            if (!dob.HasValue) return false;

            var today = DateTime.Today;
            var age = today.Year - dob.Value.Year;
            if (dob.Value.Date > today.AddYears(-age)) age--;

            return age < MinimumEmployeeAge;
        }

        // Normalize a mobile number for comparison: digits only, leading zeros
        // removed, so "7604900125" and "07604900125" compare as equal.
        private static string NormalizeMobile(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return string.Empty;
            return new string(number.Where(char.IsDigit).ToArray()).TrimStart('0');
        }

        // True when both numbers are present and refer to the same mobile number.
        private static bool AreMobileNumbersSame(string phone, string altPhone)
        {
            var p = NormalizeMobile(phone);
            var a = NormalizeMobile(altPhone);
            return p.Length > 0 && p == a;
        }

        // Checks that the mobile number and email are not already used by another
        // employee. Returns an error message when a duplicate is found, otherwise null.
        // Pass excludeEmployeeId on edit so the employee's own record is ignored.
        private async Task<string?> GetDuplicateContactErrorAsync(string phone, string email, int? excludeEmployeeId)
        {
            var normalizedPhone = NormalizeMobile(phone);
            var trimmedEmail = (email ?? string.Empty).Trim();

            var others = await _context.Employees
                .Where(e => !excludeEmployeeId.HasValue || e.EmployeeId != excludeEmployeeId.Value)
                .Select(e => new { e.PhoneNumber, e.Email })
                .ToListAsync();

            if (normalizedPhone.Length > 0 &&
                others.Any(o => NormalizeMobile(o.PhoneNumber) == normalizedPhone))
                return "This mobile number is already used by another employee."; 

            if (trimmedEmail.Length > 0 &&
                others.Any(o => string.Equals((o.Email ?? string.Empty).Trim(), trimmedEmail, StringComparison.OrdinalIgnoreCase)))
                return "This email is already used by another employee.";

            return null;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Maintanance(string FMode, int EntryID)
        {
            var activeUsers = _context.Employees
                                .Where(u => u.IsActive)
                               .Select(u => u.Name + " - " + u.Designation)
                                .ToList();

            ViewBag.ActiveUsers = activeUsers;

            EmployeeDetails model = new EmployeeDetails
            {
                FMode = FMode
            };

            if (FMode == "EDIT" || FMode == "VIEW")
            {
                var employee = await _context.Employees
                    .Include(e => e.ApplicationUser)
                    .FirstOrDefaultAsync(e => e.EmployeeId == EntryID);

               

                if (employee == null)
                {
                    return NotFound();
                }
                var addresses = await _context.MST_Address
     .Where(x => x.DocumentId == EntryID)
     .ToListAsync();

                var currentAddress = addresses
                    .FirstOrDefault(x => x.IsPermanentAddress == false);

                var permanentAddress = addresses
                    .FirstOrDefault(x => x.IsPermanentAddress == true);

                model = new EmployeeDetails
                {
                    FMode = FMode,
                    EmployeeId = employee.EmployeeId,
                    EmployeeName = employee.Name,
                    EmployeeCode = employee.EmployeeCode,
                    Gender = employee.Gender,
                    DOB = employee.DOB,
                    DateOfJoining = employee.DateOfJoining,

                    PhoneNumber = employee.PhoneNumber,
                    AlternateMoblieNumber = employee.AlternateMobileNumber,
                    Email = employee.Email,

                    ApplicationUserId = employee.ApplicationUserId,
                    UserId = employee.ApplicationUserId,
                    ReportingHeadMailID = employee.ReportingHeadMailID,

                    BloodGroup = employee.BloodGroup,
                    Nationality = employee.Nationality,
                    MartialStatus = employee.MaritalStatus,
                    FatherName = employee.FatherName,
                    MotherName = employee.MotherName,
                    SpouseName = employee.SpouseName,
                    NumberOfDependents = employee.NumberOfDependents ?? 0,

                    AadhaarNumber = employee.AadhaarNumber,
                    PanNumber = employee.PANNumber,
                    PassportNumber = employee.PassportNumber,
                    PassportExpiryDate = employee.PassportExpiryDate,
                    DrivingLicenseNumber = employee.DrivingLicenseNumber,

                    EmergencyContactName = employee.EmergencyContactName,
                    EmergencyContactRelation = employee.EmergencyContactRelation,
                    EmergencyContactNumber = employee.EmergencyContactNumber,
                    EmergencyAlternateNumber = employee.EmergencyAlternateNumber,

                    Designation = employee.Designation,

                    // ✅ CURRENT ADDRESS
                    CurrentAddress = currentAddress == null ? null : new CurrentAddressDetails
                    {
                        AddressId = currentAddress?.AddressId ?? 0,
                        Address1 = currentAddress.Address1 ?? "",
                        Address2 = currentAddress.Address2 ?? "",
                        City = currentAddress.City ?? "",
                        StateID = currentAddress?.StateId ?? 0 ,
                        Country = currentAddress.Country ?? "",
                        Pincode = currentAddress.Pincode ?? "",
                        IsPermanentAddress = false
                    },

                    // ✅ PERMANENT ADDRESS
                    PermanentAddress = permanentAddress == null ? null : new PermanentAddressDetails
                    {
                        AddressId = permanentAddress?.AddressId ?? 0,
                        Address1 = permanentAddress.Address1 ?? "",
                        Address2 = permanentAddress.Address2,
                        City = permanentAddress.City ?? "",
                        StateID = permanentAddress?.StateId ?? 0,
                        Country = permanentAddress.Country ?? "",
                        Pincode = permanentAddress.Pincode ?? "",
                        IsPermanentAddress = true
                    }
                };
            }

            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> SaveEmployeeDetails(EmployeeDetails model, IFormFile Photo)
        {
            
            if (string.IsNullOrWhiteSpace(model.EmployeeName))
                return Json(new { success = false, message = "Employee name is required." });

            if (IsUnderMinimumAge(model.DOB))
                return Json(new { success = false, message = $"Employee must be at least {MinimumEmployeeAge} years old. Please check the Date of Birth." });

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                return Json(new { success = false, message = "Phone number is required." });

            if (AreMobileNumbersSame(model.PhoneNumber, model.AlternateMoblieNumber))
                return Json(new { success = false, message = "Personal Mobile Number and Alternate Mobile Number cannot be the same." });

            if (!string.IsNullOrWhiteSpace(model.Email) &&
                !new EmailAddressAttribute().IsValid(model.Email))
                return Json(new { success = false, message = "Invalid email address." });

            var duplicateContactError = await GetDuplicateContactErrorAsync(model.PhoneNumber, model.Email, null);
            if (duplicateContactError != null)
                return Json(new { success = false, message = duplicateContactError });

            if (model.CurrentAddress == null)
                return Json(new { success = false, message = "Current address is required." });

            if (string.IsNullOrWhiteSpace(model.CurrentAddress.Address1))
                return Json(new { success = false, message = "Current address line 1 is required." });

            if (model.CurrentAddress.Pincode == null)
                return Json(new { success = false, message = "Invalid current address pincode." });

            if (!model.CurrentAddress.IsPermanentAddress)
            {
                if (model.PermanentAddress == null)
                    return Json(new { success = false, message = "Permanent address is required." });

                if (string.IsNullOrWhiteSpace(model.PermanentAddress.Address1))
                    return Json(new { success = false, message = "Permanent address line 1 is required." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
              
                var applicationUser = new ApplicationUser
                {
                    UserName = model.EmployeeName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    IsActive = true
                };

                var defaultPassword = _configuration["AppSettings:DefaultUserPassword"];

                var userResult = await _userManager.CreateAsync(applicationUser, defaultPassword);

                if (!userResult.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = string.Join(", ", userResult.Errors.Select(e => e.Description))
                    });
                }

                var roleResult = await _userManager.AddToRoleAsync(
                    applicationUser,
                    Roles.RoleType.Employee.ToString()
                );

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(applicationUser);
                    return Json(new { success = false, message = "Failed to assign employee role." });
                }

               
                string photoPath = "/images/no-image.png";

                if (Photo != null)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png" };

                    if (!allowedTypes.Contains(Photo.ContentType))
                        return Json(new { success = false, message = "Only JPG and PNG images are allowed." });

                    if (Photo.Length > 2 * 1024 * 1024)
                        return Json(new { success = false, message = "Photo size should not exceed 2MB." });

                    var fileName = $"{model.EmployeeCode}.jpg";
                    var uploadPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads/employees"
                    );

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await Photo.CopyToAsync(stream);

                    photoPath = "/uploads/employees/" + fileName;
                }

              
                var employee = new Employee
                {
                    EmployeeCode = model.EmployeeCode,
                    Name = model.EmployeeName,
                    Gender = model.Gender,
                    DOB = model.DOB ?? DateTime.Now,
                    BloodGroup = model.BloodGroup,
                    Nationality = model.Nationality,
                    MaritalStatus = model.MartialStatus,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    FatherName = model.FatherName,
                    MotherName = model.MotherName,
                    SpouseName = model.SpouseName,
                    AadhaarNumber = model.AadhaarNumber,
                    PANNumber = model.PanNumber,
                    PassportNumber = model.PassportNumber,
                    PassportExpiryDate = model.PassportExpiryDate,
                    NumberOfDependents = model.NumberOfDependents,
                    DrivingLicenseNumber = model.DrivingLicenseNumber,
                    Designation = model.Designation,
                    ImageUrl = photoPath,
                    DateOfJoining = model.DateOfJoining,
                    ReportingHeadMailID = model.ReportingHeadMailID,
                    AlternateMobileNumber = model.AlternateMoblieNumber,
                    EmergencyContactName = model.EmergencyContactName,
                    EmergencyContactNumber = model.EmergencyContactNumber,
                    EmergencyContactRelation = model.EmergencyContactRelation,
                    EmergencyAlternateNumber = model.EmergencyAlternateNumber, 
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    ApplicationUserId = applicationUser.Id
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                if (employee.EmployeeId <= 0)
                    throw new Exception("Employee ID not generated.");

               
                var currentAddress = new Address
                {
                    Address1 = model.CurrentAddress.Address1,
                    Address2 = model.CurrentAddress.Address2,
                    City = model.CurrentAddress.City,
                    StateId = model.CurrentAddress.StateID,
                    Country = model.CurrentAddress.Country ?? "India",
                    Pincode = model.CurrentAddress.Pincode,
                    DocumentType = "MST_Employee",
                    DocumentId = employee.EmployeeId,
                    IsPermanentAddress = false
                };

                _context.MST_Address.Add(currentAddress);

                
                Address permanentAddress;

                if (model.CurrentAddress.IsPermanentAddress)
                {
                    permanentAddress = new Address
                    {
                        Address1 = model.CurrentAddress.Address1,
                        Address2 = model.CurrentAddress.Address2,
                        City = model.CurrentAddress.City,
                        StateId = model.CurrentAddress.StateID,
                        Country = model.CurrentAddress.Country ?? "India",
                        Pincode = model.CurrentAddress.Pincode,
                        DocumentType = "MST_Employee",
                        DocumentId = employee.EmployeeId,
                        IsPermanentAddress = true
                    };
                }
                else
                {
                    permanentAddress = new Address
                    {
                        Address1 = model.PermanentAddress.Address1,
                        Address2 = model.PermanentAddress.Address2,
                        City = model.PermanentAddress.City,
                        StateId = model.PermanentAddress.StateID,
                        Country = model.PermanentAddress.Country ?? "India",
                        Pincode = model.PermanentAddress.Pincode,
                        DocumentType = "MST_Employee",
                        DocumentId = employee.EmployeeId,
                        IsPermanentAddress = true
                    };
                }

                if (string.IsNullOrWhiteSpace(model.CurrentAddress.Country))
                    return Json(new { success = false, message = "Please select current address country." });

                if (!model.CurrentAddress.IsPermanentAddress &&
                    string.IsNullOrWhiteSpace(model.PermanentAddress.Country))
                {
                    return Json(new { success = false, message = "Please select permanent address country." });
                }


                _context.MST_Address.Add(permanentAddress);
                await _context.SaveChangesAsync();

               
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Employee and address saved successfully."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return Json(new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPost]
public async Task<IActionResult> UpdateEmployeeDetails(EmployeeDetails model, IFormFile Photo)
{
   
    if (model.EmployeeId <= 0)
        return Json(new { success = false, message = "Invalid employee." });

    if (string.IsNullOrWhiteSpace(model.EmployeeName))
        return Json(new { success = false, message = "Employee name is required." });

    if (IsUnderMinimumAge(model.DOB))
        return Json(new { success = false, message = $"Employee must be at least {MinimumEmployeeAge} years old. Please check the Date of Birth." });

    if (string.IsNullOrWhiteSpace(model.PhoneNumber))
        return Json(new { success = false, message = "Phone number is required." });

    if (AreMobileNumbersSame(model.PhoneNumber, model.AlternateMoblieNumber))
        return Json(new { success = false, message = "Personal Mobile Number and Alternate Mobile Number cannot be the same." });

    if (!string.IsNullOrWhiteSpace(model.Email) &&
        !new EmailAddressAttribute().IsValid(model.Email))
        return Json(new { success = false, message = "Invalid email address." });

    var duplicateContactError = await GetDuplicateContactErrorAsync(model.PhoneNumber, model.Email, model.EmployeeId);
    if (duplicateContactError != null)
        return Json(new { success = false, message = duplicateContactError });

    if (model.CurrentAddress == null)
        return Json(new { success = false, message = "Current address is required." });

    if (string.IsNullOrWhiteSpace(model.CurrentAddress.Address1))
        return Json(new { success = false, message = "Current address line 1 is required." });

    if (string.IsNullOrWhiteSpace(model.CurrentAddress.Country))
        return Json(new { success = false, message = "Please select current address country." });

    if (!model.CurrentAddress.IsPermanentAddress &&
        string.IsNullOrWhiteSpace(model.PermanentAddress?.Country))
        return Json(new { success = false, message = "Please select permanent address country." });

    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
     
        var employee = await _context.Employees
            .FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId);

        if (employee == null)
            return Json(new { success = false, message = "Employee not found." });

              
                var applicationUser = await _userManager
             .FindByIdAsync(employee.ApplicationUserId.ToString());


                if (applicationUser == null)
            return Json(new { success = false, message = "User account not found." });

        applicationUser.UserName = model.EmployeeName;
        applicationUser.Email = model.Email;
        applicationUser.PhoneNumber = model.PhoneNumber;

        await _userManager.UpdateAsync(applicationUser);

      
        if (Photo != null)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png" };

            if (!allowedTypes.Contains(Photo.ContentType))
                return Json(new { success = false, message = "Only JPG and PNG images are allowed." });

            if (Photo.Length > 2 * 1024 * 1024)
                return Json(new { success = false, message = "Photo size should not exceed 2MB." });

            var fileName = $"{employee.EmployeeCode}.jpg";
            var uploadPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/employees"
            );

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await Photo.CopyToAsync(stream);

            employee.ImageUrl = "/uploads/employees/" + fileName;
        }

       
        employee.Name = model.EmployeeName;
        employee.Gender = model.Gender;
        employee.DOB = model.DOB ?? DateTime.Now;
        employee.BloodGroup = model.BloodGroup;
        employee.Nationality = model.Nationality;
        employee.MaritalStatus = model.MartialStatus;
        employee.PhoneNumber = model.PhoneNumber;
        employee.Email = model.Email;
        employee.FatherName = model.FatherName;
        employee.MotherName = model.MotherName;
        employee.SpouseName = model.SpouseName;
        employee.AadhaarNumber = model.AadhaarNumber;
        employee.PANNumber = model.PanNumber;
        employee.PassportNumber = model.PassportNumber;
        employee.PassportExpiryDate = model.PassportExpiryDate;
        employee.NumberOfDependents = model.NumberOfDependents;
        employee.DrivingLicenseNumber = model.DrivingLicenseNumber;
        employee.Designation = model.Designation;
        employee.DateOfJoining = model.DateOfJoining;
        employee.ReportingHeadMailID = model.ReportingHeadMailID;
        employee.AlternateMobileNumber = model.AlternateMoblieNumber;
        employee.EmergencyContactName = model.EmergencyContactName;
        employee.EmergencyContactNumber = model.EmergencyContactNumber;
        employee.EmergencyContactRelation = model.EmergencyContactRelation;
        employee.EmergencyAlternateNumber = model.EmergencyAlternateNumber;

        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();

      
        var addresses = await _context.MST_Address
            .Where(x => x.DocumentType == "MST_Employee"
                     && x.DocumentId == employee.EmployeeId)
            .ToListAsync();

        var currentAddress = addresses.FirstOrDefault(x => !x.IsPermanentAddress);
        var permanentAddress = addresses.FirstOrDefault(x => x.IsPermanentAddress);

        var isNewCurrent = currentAddress == null;
        if (isNewCurrent)
        {
            currentAddress = new Address
            {
                DocumentType = "MST_Employee",
                DocumentId = employee.EmployeeId,
                IsPermanentAddress = false
            };
        }

        currentAddress.Address1 = model.CurrentAddress.Address1;
        currentAddress.Address2 = model.CurrentAddress.Address2;
        currentAddress.City = model.CurrentAddress.City;
        currentAddress.StateId = model.CurrentAddress.StateID;
        currentAddress.Country = model.CurrentAddress.Country;
        currentAddress.Pincode = model.CurrentAddress.Pincode;

        if (isNewCurrent)
            _context.MST_Address.Add(currentAddress);

        var isNewPermanent = permanentAddress == null;
        if (isNewPermanent)
        {
            permanentAddress = new Address
            {
                DocumentType = "MST_Employee",
                DocumentId = employee.EmployeeId,
                IsPermanentAddress = true
            };
        }

        if (model.CurrentAddress.IsPermanentAddress)
        {
            permanentAddress.Address1 = model.CurrentAddress.Address1;
            permanentAddress.Address2 = model.CurrentAddress.Address2;
            permanentAddress.City = model.CurrentAddress.City;
            permanentAddress.StateId = model.CurrentAddress.StateID;
            permanentAddress.Country = model.CurrentAddress.Country;
            permanentAddress.Pincode = model.CurrentAddress.Pincode;
        }
        else
        {
            permanentAddress.Address1 = model.PermanentAddress.Address1;
            permanentAddress.Address2 = model.PermanentAddress.Address2;
            permanentAddress.City = model.PermanentAddress.City;
            permanentAddress.StateId = model.PermanentAddress.StateID;
            permanentAddress.Country = model.PermanentAddress.Country;
            permanentAddress.Pincode = model.PermanentAddress.Pincode;
        }

        if (isNewPermanent)
            _context.MST_Address.Add(permanentAddress);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return Json(new
        {
            success = true,
            message = "Employee details updated successfully."
        });
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();

        return Json(new
        {
            success = false,
            message = ex.InnerException?.Message ?? ex.Message
        });
    }
}
        [HttpPost]
public async Task<IActionResult> Delete(int id)
{
    var employee = await _context.Employees.FindAsync(id);

    if (employee == null)
    {
        return Json(new { success = false, message = "Employee not found." });
    }

    _context.Employees.Remove(employee);
    await _context.SaveChangesAsync();

    return Json(new { success = true });
}





        //// GET: Employee/Create
        //public IActionResult Create()
        //{
        //    ViewBag.ActiveUsers = _context.Users.Where(x => x.IsActive).ToList();
        //    ViewBag.States = _context.MST_State.Where(x => x.IsActive).ToList();
        //    return View();
        //}


        //// POST: Employee/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeCode,Name,Email,PhoneNumber,DOB,Gender,Designation,ImageUrl,DateOfJoining,Address,ReportingHeadMailID")] Employee employee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var applicationUser = new ApplicationUser
        //        {
        //            UserName = employee.Email,
        //            Email = employee.Email,
        //            IsActive = true,
        //            PhoneNumber = employee.PhoneNumber,
        //        };
        //        var defaultPassword = _configuration["AppSettings:DefaultUserPassword"];
        //        var result = await _userManager.CreateAsync(applicationUser, defaultPassword);

        //        if (result.Succeeded)
        //        {
        //            var roleResult = await _userManager.AddToRoleAsync(applicationUser, Roles.RoleType.Employee.ToString());

        //            if (!roleResult.Succeeded)
        //            {
        //                foreach (var error in roleResult.Errors)
        //                {
        //                    ModelState.AddModelError(string.Empty, error.Description);
        //                }

        //                await _userManager.DeleteAsync(applicationUser);
        //                return View(employee);
        //            }

        //            try
        //            {
        //                employee.IsActive = true;
        //                employee.ApplicationUserId = applicationUser.Id;
        //                employee.ImageUrl = _configuration["AppSettings:ImagePath"] + "/" + employee.EmployeeCode + ".jpg";
        //                if (employee.ImageUrl == null)
        //                {
        //                    employee.ImageUrl = "https://www.freeiconspng.com/thumbs/no-image-icon/no-image-icon-6.png";
        //                }
        //                if (employee.DateOfJoining == null)
        //                {
        //                    employee.DateOfJoining = DateTime.Now;
        //                }
        //                _context.Add(employee);
        //                await _context.SaveChangesAsync();

        //                return RedirectToAction(nameof(Index));
        //            }
        //            catch (Exception ex)
        //            {
        //                ModelState.AddModelError(string.Empty, "An error occurred while saving the employee. Please try again.");
        //            }

        //            return RedirectToAction(nameof(Index));
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (var key in ModelState.Keys)
        //        {
        //            if (ModelState[key].Errors.Count > 0)
        //            {
        //                ModelState.AddModelError(string.Empty, $"{key} is required.");
        //            }
        //        }

        //        ModelState.AddModelError(string.Empty, "Please fill all required fields.");
        //    }
        //    ViewBag.ActiveUsers = _context.Users.Where(x => x.IsActive == true).ToList();
        //    return View(employee);
        //}

        //// GET: Employee/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    ViewBag.ActiveUsers = _context.Users;
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var employee = await _context.Employees.FindAsync(id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", employee.ApplicationUserId);
        //    return View(employee);
        //}

        //// POST: Employee/Edit/5

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,EmployeeCode,Name,Email,PhoneNumber,Gender,Designation,ImageUrl,Address,ApplicationUserId,IsActive")] Employee employee)
        //{
        //    if (id != employee.EmployeeId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        employee.ApplicationUserId = _context.Employees.Where(x => x.EmployeeId == employee.EmployeeId).Select(x => x.ApplicationUserId).FirstOrDefault();
        //        if (employee.ImageUrl == null)
        //        {
        //            employee.ImageUrl = "https://www.freeiconspng.com/thumbs/no-image-icon/no-image-icon-6.png";
        //        }

        //        try
        //        {
        //            _context.Update(employee);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!EmployeeExists(employee.EmployeeId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", employee.ApplicationUserId);
        //    return View(employee);
        //}

        //// GET: Employee/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var employee = await _context.Employees
        //        .Include(e => e.ApplicationUser)
        //        .FirstOrDefaultAsync(m => m.EmployeeId == id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(employee);
        //}

        //// POST: Employee/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var employee = await _context.Employees.FindAsync(id);
        //    if (employee != null)
        //    {
        //        _context.Employees.Remove(employee);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
