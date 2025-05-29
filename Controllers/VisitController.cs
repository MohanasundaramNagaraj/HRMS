using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using SparkHRMS.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using SparkHRMS.Data.Setting;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using SparkHRMS.ViewModels;

namespace SparkHRMS.Controllers
{
    public class VisitController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly Utility utility;

        private readonly IBackgroundJobClient _backgroundJobClient;
        public VisitController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration, Utility utility,  IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            this.utility = utility;
            _backgroundJobClient = backgroundJobClient;
        }

        public IActionResult Index(int? YearId, int? MonthId)
        {
            var data = _context.VisitorEntries.AsQueryable();
            if (YearId.HasValue)
                data = data.Where(x => x.EntryDate.Year == YearId);
            if (MonthId.HasValue)
                data = data.Where(x => x.EntryDate.Month == MonthId);

            ViewBag.SelectedYearId = YearId;
            ViewBag.SelectedMonth = MonthId;

            ViewBag.YearList = _context.Year.ToList();

            return View(data.ToList());
        }

        [HttpPost]
        public IActionResult GetVisitors([FromForm] DataTableRequest request, int? YearId, int? MonthId)
        {
            var query = _context.VisitorEntries.AsQueryable();

            if (YearId.HasValue)
                query = query.Where(x => x.EntryDate.Year == YearId);

            if (MonthId.HasValue)
                query = query.Where(x => x.EntryDate.Month == MonthId);

            var totalRecords = query.Count();

            string searchText = request.Search.Value;
            if (!string.IsNullOrEmpty(searchText)){
                query = query.Where(x =>
                x.PassNo.Contains(searchText) ||
                x.VisitorName.Contains(searchText) ||
                x.CompanyName.Contains(searchText) ||
                x.ContactNumber.Contains(searchText) ||
                x.PersonToMeet.Contains(searchText) ||
                x.Department.Contains(searchText) ||
                x.InTime.ToString().Contains(searchText) ||
                x.OutTime.ToString().Contains(searchText)
                );
            }
            
            var data = query
                .OrderByDescending(x => x.EntryDate)
                .Skip(request.Start)
                .Take(request.Length)
                .Select(x => new
                {
                    passNo = x.PassNo,
                    visitorName = x.VisitorName,
                    company = x.CompanyName,
                    phoneNumber = x.ContactNumber,
                    personToMeet = x.PersonToMeet,
                    department = x.Department,
                    additionalVisitorsCount = x.AdditionalVisitorCount,
                    entryDateTime = x.InTime.ToString("dd-MM-yyyy HH:mm"),
                    exitDateTime = x.OutTime != null ? x.OutTime.Value.ToString("dd-MM-yyyy HH:mm") : "",
                    action = $@"
                                <button class='btn btn-sm btn-primary mr-2' onclick='redirectToMaintanance(""VIEW"", {x.Id})'>View</button> "
                                + (!x.OutTime.HasValue ? $@"
                                <button class='btn btn-sm btn-warning mr-2' onclick='redirectToMaintanance(""EDIT"", {x.Id})'>Edit</button>
                                <button class='btn btn-sm btn-danger mr-2' onclick='outEntry({x.Id})'>Out</button> " : "")
                                + $@" <a class='btn btn-sm btn-success' href='/Visit/Print/{x.Id}' target='_blank'>Print</a>"

                })
                .ToList();

            return Json(new
            {
                draw = request.Draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }


        public async Task<IActionResult> MaintananceAsync(string FMode, int? EntryID)
        {
            VisitorEntry entry = new VisitorEntry();
            string documentNumber = "";
            if (FMode == "ADD")
            {
                utility.GenerateDocumentNumber("Visitor_Pass", false, out documentNumber);
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();
                var emp = _context.Employees.Where(x => x.ApplicationUserId == user.Id).FirstOrDefault();
                
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");

                ViewBag.IsUserAdmin = isAdmin || isSuperAdmin;

                entry.EntryDate = DateTime.Now;
                entry.GateInCharge = user.UserName;
                entry.PassNo = documentNumber;
            }
            else
            {
                entry = await _context.VisitorEntries.Where(x => x.Id == EntryID).FirstOrDefaultAsync();
            }

            ViewBag.FMode = FMode;
            ViewBag.EntryID = EntryID;

            ViewBag.Locations = _configuration.GetSection("VisitorPassSettings:GateLocationsForVisitorPass").Get<List<string>>();
            
            ViewBag.EmployeeList = _context.Employees.ToList();
            return View(entry);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVisitorEntry([FromBody] VisitorEntry model)
        {
            if (ModelState.IsValid)
            {
                model.EntryDate = model.EntryDate == default ? DateTime.Now : model.EntryDate;
                model.InTime = DateTime.Now;
                model.OutTime = null;
                _context.VisitorEntries.Add(model);
                await _context.SaveChangesAsync();
                string documentNumber;
                utility.GenerateDocumentNumber("Visitor_Pass", true, out documentNumber);
                return Ok();
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVisitorEntry([FromBody] VisitorEntry model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _context.VisitorEntries.FindAsync(model.Id);
            if (existing == null)
                return NotFound("Visitor entry not found");

            // Update properties
            existing.PassNo = model.PassNo;
            existing.Location = model.Location;
            existing.GateInCharge = model.GateInCharge;
            existing.ContactNumber = model.ContactNumber;
            existing.VisitorName = model.VisitorName;
            existing.Gender = model.Gender;
            existing.VisitorCategory = model.VisitorCategory;
            existing.CompanyName = model.CompanyName;
            existing.PurposeOfVisit = model.PurposeOfVisit;
            existing.PersonToMeet = model.PersonToMeet;
            existing.Department = model.Department;
            existing.IDProofType = model.IDProofType;
            existing.IDProofNumber = model.IDProofNumber;
            existing.Address = model.Address;
            existing.VisitorImageUrl = model.VisitorImageUrl;
            existing.VehicleNumber = model.VehicleNumber;
            existing.AdditionalVisitorCount = model.AdditionalVisitorCount;
            existing.AdditionalVisitorNames = model.AdditionalVisitorNames;
            existing.RecentVisits = model.RecentVisits;

            _context.VisitorEntries.Update(existing);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VisitorEntry>> GetVisitorEntry(int id)
        {
            var entry = await _context.VisitorEntries.FindAsync(id);
            if (entry == null)
                return NotFound();
            return Ok(entry);
        }

        public IActionResult Print(int id)
        {
            var entry = _context.VisitorEntries.Find(id);
            return View(entry);
        }

        [HttpGet]
        public IActionResult GetVisitorByContact(string contactNumber)
        {
            if (string.IsNullOrWhiteSpace(contactNumber))
                return BadRequest("Contact number is required.");

            var visitor = _context.VisitorEntries
                .FirstOrDefault(v => v.ContactNumber.Contains(contactNumber));

            if (visitor == null)
                return NotFound();

            visitor.RecentVisits = _context.VisitorEntries.Where(x => x.ContactNumber == contactNumber).Count();
               

            return Json(new
            {
                VisitorName = visitor.VisitorName,
                gender = visitor.Gender,
                visitorCategory = visitor.VisitorCategory,
                companyName = visitor.CompanyName,
                purposeOfVisit = visitor.PurposeOfVisit,
                personToMeet = visitor.PersonToMeet,
                department = visitor.Department,
                idProofType = visitor.IDProofType,
                idProofNumber = visitor.IDProofNumber,
                address = visitor.Address,
                visitorImageUrl = visitor.VisitorImageUrl,
                vehicleNumber = visitor.VehicleNumber,
                additionalVisitorCount = visitor.AdditionalVisitorCount,
                additionalVisitorNames = visitor.AdditionalVisitorNames,
                recentVisits = visitor.RecentVisits
            });
        }

        [HttpPost]
        public async Task<IActionResult> OutEntry(int id)
        {

            var existing = await _context.VisitorEntries.FindAsync(id);
            if (existing == null)
                return NotFound("Visitor entry not found");
            existing.OutTime = DateTime.Now;

            _context.VisitorEntries.Update(existing);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SaveVisitorImage(IFormFile VisitorImage, string CompanyName, string VisitorName)
        {
            if (VisitorImage == null || VisitorImage.Length == 0)
                return BadRequest("No image file provided.");

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string folderName = $"{today}_{Sanitize(CompanyName)}_{Sanitize(VisitorName)}";
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "VisitorImages", folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(VisitorImage.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await VisitorImage.CopyToAsync(fileStream);
            }

            string publicPath = $"/VisitorImages/{folderName}/{uniqueFileName}";

            return Ok(new { imagePath = publicPath });
        }

        [HttpPost]
        public IActionResult SaveCapturedImage([FromBody] VisitorImageUploadDto dto)
        {
            if (string.IsNullOrEmpty(dto.Base64Image))
                return BadRequest("Image data is required.");

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string folderName = $"{today}_{Sanitize(dto.CompanyName)}_{Sanitize(dto.VisitorName)}";
            string folderPath = Path.Combine("wwwroot", "VisitorImages", folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string base64 = Regex.Replace(dto.Base64Image, @"^data:image\/[a-zA-Z]+;base64,", "");
            byte[] imageBytes = Convert.FromBase64String(base64);
            string fileName = Guid.NewGuid() + ".jpg";
            string fullPath = Path.Combine(folderPath, fileName);

            System.IO.File.WriteAllBytes(fullPath, imageBytes);

            string publicUrl = $"/VisitorImages/{folderName}/{fileName}";
            return Ok(new { imagePath = publicUrl });
        }

        private string Sanitize(string input)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }
            return input.Replace(" ", "_");
        }

    }
}

public class VisitorImageUploadDto
{
    public string Base64Image { get; set; }
    public string CompanyName { get; set; }
    public string VisitorName { get; set; }
}
