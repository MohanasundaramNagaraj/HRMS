using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using SparkHRMS.ViewModels;

namespace SparkHRMS.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminAttendanceViewerController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminAttendanceViewerController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index(DateTime? date = null)
        {
            var selectedDate = date ?? DateTime.Today;
            var attendances = await _context.EmployeeAttendance
                .Include(e => e.Employee) // Assuming Employee is a navigation property
                .Where(e => e.CheckInTime.Date == selectedDate)
                .OrderBy(e => e.CheckInTime)
                .ToListAsync();

            var attendanceDtos = attendances.Select(a => new EmployeeAttendanceDto
            {
                EmployeeName = a.Employee.UserName,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                WorkingHours = a.CheckOutTime.HasValue
                               ? string.Format("{0:%h} hours {0:%m} mins", a.CheckOutTime.Value - a.CheckInTime)
                               : "N/A"
            }).ToList();

            ViewBag.SelectedDate = selectedDate;

            return View(attendanceDtos);
        }
    }
}
