using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Utilities;
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
        private readonly IConfiguration _configuration;

        public EmployeeAttendanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(int? EmployeeID = null, int? Month = null)
        {
            var emp = new Employee();
            if (EmployeeID == null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();
                emp = _context.Employees.Where(x => x.ApplicationUserId == user.Id).FirstOrDefault();
                if (emp == null)
                {
                    emp = _context.Employees.FirstOrDefault();
                }
            }
            else
            {
                emp = _context.Employees.Where(x => x.EmployeeId == EmployeeID).FirstOrDefault();
            }

            // Fetch Employee Details
            var employeeDetails = new EmployeeDetailsDto
            {
                EmployeeId = emp.EmployeeId,
                Name = emp.Name,
                PhoneNumber = emp.PhoneNumber,
                Email = emp.Email,
                ImageUrl = emp.ImageUrl,
                DateOfJoining = emp.DateOfJoining,
                Address = emp.Address,
                Designation = emp.Designation,
                EmpCode = emp.EmployeeCode
            };

            // Fetch This Month's Check-in/Check-out Summary
            var today = DateTime.Today;
            var year = today.Year;
            if (Month.HasValue)
            {
                year = Month.Value == 1 ? year - 1 : year; // For January, go to the previous year (if needed)
            }

            var startOfMonth = Month.HasValue ? new DateTime(year, Month.Value, 1) : new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Get the last day of the month

            var attendanceRecords = await _context.EmployeeAttendance
                .Where(e => e.EmployeeId == emp.EmployeeId && e.CheckInTime.Date >= startOfMonth)
                .OrderBy(e => e.CheckInTime)
                .ToListAsync();

            var dailyAttendanceRecords = new List<EmployeeAttendanceDto>();
            TimeSpan totalWorkingTime = TimeSpan.Zero;

            var allDatesInMonth = Enumerable.Range(0, DateTime.DaysInMonth(today.Year, today.Month))
                                   .Select(day => new DateTime(today.Year, today.Month, day + 1))
                                   .ToList();

            // Fetch holidays from the database or your holidays table
            var holidays = await _context.Holiday
                                .Where(h => h.Date >= startOfMonth && h.Date <= today)
                                .Select(h => h.Date.Date)
                                .ToListAsync();

            foreach (var date in allDatesInMonth)
            {
                var attendanceRecord = attendanceRecords.FirstOrDefault(a => a.CheckInTime.Date == date);
                TimeSpan? workingTime = null;

                AttendanceStatus status = AttendanceStatus.Absent; // Default status
                if (holidays.Contains(date))
                {
                    status = AttendanceStatus.Holiday; // If the date is a holiday
                }
                else if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    status = AttendanceStatus.Weekend; // If the date is a Sunday
                }
                else if (attendanceRecord != null)
                {
                    if (attendanceRecord.CheckOutTime.HasValue)
                    {
                        workingTime = attendanceRecord.CheckOutTime.Value - attendanceRecord.CheckInTime;
                        totalWorkingTime += workingTime.Value;

                        if (workingTime.Value.TotalHours >= Convert.ToInt32(_configuration["AttendanceSettings:FullDayThreshold"]))
                        {
                            status = AttendanceStatus.Present; // Full Day Present
                        }
                        else if (workingTime.Value.TotalHours >= Convert.ToInt32(_configuration["AttendanceSettings:HalfDayThreshold"])
                            && workingTime.Value.TotalHours <= Convert.ToInt32(_configuration["AttendanceSettings:PermissionNeededThreshold"]))
                        {
                            status = AttendanceStatus.PermissionNeeded; // Permission Needed
                        }
                        else if (workingTime.Value.TotalHours >= Convert.ToInt32(_configuration["AttendanceSettings:HalfDayThreshold"]))
                        {
                            status = AttendanceStatus.HalfDay; // Half Day Present
                        }
                        else
                        {
                            status = AttendanceStatus.Absent;
                        }
                    }
                    else
                    {
                        // If checked in but not checked out
                        status = AttendanceStatus.PendingCheckOut; // Present (Pending Checkout)
                    }
                }

                // Add attendance record with status
                dailyAttendanceRecords.Add(new EmployeeAttendanceDto
                {
                    Id = attendanceRecord?.Id ?? 0,
                    EmployeeId = emp.EmployeeId,
                    EmployeeName = emp.Name,
                    CheckInDateTime = attendanceRecord?.CheckInTime,
                    CheckOutDateTime = attendanceRecord?.CheckOutTime,
                    IP = attendanceRecord?.CheckinMadeSystemIP,
                    CheckInPosition = attendanceRecord?.CheckInPosition,
                    CheckOutPosition = attendanceRecord?.CheckOutPosition,
                    Date = date,
                    CheckInTimeInString = attendanceRecord?.CheckInTime.ToString("hh:mm tt", CultureInfo.InvariantCulture) ?? "Not Checked In",
                    CheckOutTimeInString = attendanceRecord?.CheckOutTime?.ToString("hh:mm tt", CultureInfo.InvariantCulture) ?? "Not Checked Out",
                    WorkingHours = workingTime.HasValue
                                   ? string.Format("{0:%h} hours {0:%m} mins", workingTime.Value)
                                   : string.Empty,
                    Status = status 
                });
            }

            // Get check-in/check-out locations
            foreach (var attendance in dailyAttendanceRecords)
            {
                attendance.CheckInLocation = await Utility.GetLocationFromCoordinates(attendance.CheckInPosition);
                attendance.CheckOutLocation = await Utility.GetLocationFromCoordinates(attendance.CheckOutPosition);
            };

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

            ViewBag.EmployeeList = _context.Employees.ToList();
            ViewBag.SelectedEmployeeID = employeeDetails.EmployeeId;
            return View(response);
        }

        // POST: Attendance/CheckIn
        [HttpPost]
        public async Task<IActionResult> CheckIn(string CheckInPosition)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var today = DateTime.Today;
            var emp = _context.Employees.Where(x => x.ApplicationUserId == user.Id).FirstOrDefault();
            var existingCheckIn = _context.EmployeeAttendance
                .FirstOrDefault(c => c.EmployeeId == emp.EmployeeId && c.CheckInTime.Date == today);

            if (existingCheckIn != null)
            {
                return BadRequest("You have already checked in today.");
            }

            var checkIn = new EmployeeAttendance
            {
                EmployeeId = emp.EmployeeId,
                CheckInTime = DateTime.Now,
                CheckinMadeSystemIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                CheckInPosition = CheckInPosition
            };

            _context.EmployeeAttendance.Add(checkIn);
            await _context.SaveChangesAsync();

            return Ok("Check-in successful.");
        }

        // POST: Attendance/CheckOut
        [HttpPost]
        public async Task<IActionResult> CheckOut(string CheckOutPosition)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var emp = _context.Employees.Where(x => x.ApplicationUserId == user.Id).FirstOrDefault();

            var today = DateTime.Today;
            var checkInRecord = _context.EmployeeAttendance
                                .Where(e => e.EmployeeId == emp.EmployeeId && e.CheckInTime.Date == today && e.CheckOutTime == null)
                                .FirstOrDefault();

            if (checkInRecord == null)
            {
                return BadRequest("You have not checked in today or have already checked out.");
            }

            checkInRecord.CheckOutTime = DateTime.Now;
            checkInRecord.CheckOutPosition = CheckOutPosition;
            await _context.SaveChangesAsync();

            return Ok("Check-out successful.");
        }

    }
}
