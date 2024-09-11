using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace SparkHRMS.Controllers
{
    [Authorize]
    public class EmployeeAttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeAttendanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Fetch Employee Details
            var employeeDetails = new EmployeeDetailsDto
            {
                Name = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                //ImageUrl = user.ImageUrl,
                //DateOfJoining = user.DateOfJoining,
                //Address = user.Address,
                //Designation = "Software Developer",
                //EmpCode = "SPK-001"
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
        // POST: Attendance/CheckIn
        [HttpPost]
        public async Task<IActionResult> CheckIn()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var today = DateTime.Today;
            var existingCheckIn = _context.EmployeeAttendance
                .FirstOrDefault(c => c.EmployeeId == user.Id && c.CheckInTime.Date == today);

            if (existingCheckIn != null)
            {
                return BadRequest("You have already checked in today.");
            }

            var checkIn = new EmployeeAttendance
            {
                EmployeeId = user.Id,
                CheckInTime = DateTime.Now
            };

            _context.EmployeeAttendance.Add(checkIn);
            await _context.SaveChangesAsync();

            return Ok("Check-in successful.");
        }

        // POST: Attendance/CheckOut
        [HttpPost]
        public async Task<IActionResult> CheckOut()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var today = DateTime.Today;
            var checkInRecord = _context.EmployeeAttendance
                                .Where(e => e.EmployeeId == user.Id && e.CheckInTime.Date == today && e.CheckOutTime == null)
                                .FirstOrDefault();

            if (checkInRecord == null)
            {
                return BadRequest("You have not checked in today or have already checked out.");
            }

            checkInRecord.CheckOutTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok("Check-out successful.");
        }

    }
}
