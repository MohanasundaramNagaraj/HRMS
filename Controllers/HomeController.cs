using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using SparkHRMS.Models;
using SparkHRMS.ViewModels;
using System.Diagnostics;
using System.Globalization;
using SparkHRMS.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SparkHRMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Fetch Employee Details
            var employeeDetails = new EmployeeDetailsDto
            {
                Name = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                DateOfJoining = user.DateOfJoining,
                Address = user.Address
            };

            // Fetch This Month's Check-in/Check-out Summary
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var attendanceRecords = await _context.EmployeeAttendance
                .Where(e => e.EmployeeId == user.Id && e.CheckInTime.Date >= startOfMonth)
                .OrderBy(e => e.CheckInTime)
                .ToListAsync();

            var dailyAttendanceRecords = new List<DailyAttendanceDto>();
            TimeSpan totalWorkingTime = TimeSpan.Zero;

            foreach (var record in attendanceRecords)
            {
                TimeSpan? workingTime = null;
                if (record.CheckOutTime.HasValue)
                {
                    workingTime = record.CheckOutTime.Value - record.CheckInTime;
                    totalWorkingTime += workingTime.Value;
                }

                dailyAttendanceRecords.Add(new DailyAttendanceDto
                {
                    Date = record.CheckInTime.Date,
                    CheckInTime = record.CheckInTime.ToString("hh:mm tt", CultureInfo.InvariantCulture),
                    CheckOutTime = record.CheckOutTime?.ToString("hh:mm tt", CultureInfo.InvariantCulture) ?? "Not Checked Out",
                    WorkingHours = workingTime.HasValue
                                   ? string.Format("{0:%h} hours {0:%m} mins", workingTime.Value)
                                   : "N/A"
                });
            }

            var monthlySummary = new EmployeeMonthlySummary
            {
                DailyAttendanceRecords = dailyAttendanceRecords,
                TotalWorkingHoursThisMonth = string.Format("{0:%d} days {0:%h} hours {0:%m} mins", totalWorkingTime)
            };

            var response = new EmployeeAttendanceResponseDto
            {
                EmployeeDetails = employeeDetails,
                MonthlySummary = monthlySummary
            };
            return View(response);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Dashboard(DateTime? date = null)
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
