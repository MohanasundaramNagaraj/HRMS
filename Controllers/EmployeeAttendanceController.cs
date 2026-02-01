using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Services;
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
        public async Task<IActionResult> Index(int? EmployeeID, int? Month, int? Year)
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

            if (emp == null) return NotFound();
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
            var year = Year.HasValue ? (int)Year : today.Year;

            var startOfMonth = Month.HasValue ? new DateTime(year, Month.Value, 1) : new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Get the last day of the month

            var attendanceRecords = await _context.EmployeeAttendance
                .Where(e => e.EmployeeId == emp.EmployeeId && e.CheckInTime.Date >= startOfMonth)
                .OrderBy(e => e.CheckInTime)
                .ToListAsync();

            var dailyAttendanceRecords = new List<EmployeeAttendanceDto>();
            TimeSpan totalWorkingTime = TimeSpan.Zero;

            var allDatesInMonth = Enumerable.Range(0, DateTime.DaysInMonth(year, Month.HasValue ? Month.Value : today.Month))
                                   .Select(day => new DateTime(year, Month.HasValue ? Month.Value : today.Month, day + 1))
                                   .ToList();

            // Fetch holidays from the database or your holidays table
            var holidays = await _context.Holiday
                                .Where(h => h.Date >= startOfMonth && h.Date <= endOfMonth)
                                .Select(h => h.Date.Date)
                                .ToListAsync();


            foreach (var date in allDatesInMonth)
            {
                var leaveRequestedDetails = await _context.LeaveRequest.Where(x => x.EmployeeId == emp.EmployeeId
                && x.StartDate.Date <= date.Date
                && x.EndDate.Date >= date.Date
               ).FirstOrDefaultAsync();

                var attendanceRecord = attendanceRecords.FirstOrDefault(a => a.CheckInTime.Date == date);
                TimeSpan? workingTime = null;

                AttendanceStatus status = AttendanceStatus.Absent; // Default status
                if (holidays.Contains(date))
                {
                    status = AttendanceStatus.Holiday; // If the date is a holiday
                }
                else if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    status = AttendanceStatus.Weekend;
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
                        else if (workingTime.Value.TotalHours >= Convert.ToInt32(_configuration["AttendanceSettings:HalfDayThreshold"]))
                        {
                            status = AttendanceStatus.HalfDay; // Half Day Present
                        }
                        else if (workingTime.Value.TotalHours >= Convert.ToInt32(_configuration["AttendanceSettings:HalfDayThreshold"])
                            && workingTime.Value.TotalHours <= Convert.ToInt32(_configuration["AttendanceSettings:PermissionNeededThreshold"]))
                        {
                            status = AttendanceStatus.PermissionNeeded; // Permission Needed
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

                if (status == AttendanceStatus.Absent && leaveRequestedDetails != null && leaveRequestedDetails.Status != "CAN-ACC")
                {
                    status = AttendanceStatus.LeaveRequested;

                    if (leaveRequestedDetails.Status == "ACC")
                    {
                        status = AttendanceStatus.LeaveRequestApproved;
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
                    CheckOutMadeSystemIP = attendanceRecord?.CheckOutMadeSystemIP,
                    // CheckInPosition = attendanceRecord?.CheckInPosition,
                    // CheckOutPosition = attendanceRecord?.CheckOutPosition,
                    Date = date,
                    CheckInTimeInString = attendanceRecord?.CheckInTime.ToString("hh:mm tt", CultureInfo.InvariantCulture) ?? "Not Checked In",
                    CheckOutTimeInString = attendanceRecord?.CheckOutTime?.ToString("hh:mm tt", CultureInfo.InvariantCulture) ?? "Not Checked Out",
                    WorkingHours = workingTime.HasValue
                                   ? string.Format("{0:%h} hours {0:%m} mins", workingTime.Value)
                                   : string.Empty,
                    Status = status,

                    IsPermission = attendanceRecord?.IsPermission ?? false,
                    PermissionStartTime = attendanceRecord?.PermissionStartTime,
                    PermissionEndTime = attendanceRecord?.PermissionEndTime,

                    IsOnDuty = attendanceRecord?.IsOnDuty ?? false,
                    DutyStartTime = attendanceRecord?.DutyStartTime,
                    DutyEndTime = attendanceRecord?.DutyEndTime,

                    IsLeave = attendanceRecord?.IsLeave ?? false,
                    IsHalfDayLeave = attendanceRecord?.IsHalfDayLeave ?? false,
                });
            }

            // Get check-in/check-out locations
            //foreach (var attendance in dailyAttendanceRecords)
            //{
            //    attendance.CheckInLocation = await Utility.GetLocationFromCoordinates(attendance.CheckInPosition);
            //    attendance.CheckOutLocation = await Utility.GetLocationFromCoordinates(attendance.CheckOutPosition);
            //};

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
            ViewBag.SelectedMonth = startOfMonth.Month;
            ViewBag.SelectedYear = startOfMonth.Year;

            ViewBag.TotalWorkingDays = dailyAttendanceRecords.Where(x => x.WorkingHours != string.Empty).Count();
            ViewBag.TotalPermissionHours = dailyAttendanceRecords.Where(x => x.IsPermission == true).Count();
            ViewBag.TotalAbsentDays = dailyAttendanceRecords.Where(x => x.IsLeave == true).Count() + (dailyAttendanceRecords.Where(x => x.IsHalfDayLeave == true).Count() * 0.5);
            ViewBag.TotalWeekendDays = dailyAttendanceRecords.Where(x => x.Status == AttendanceStatus.Weekend).Count();

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

            var machineService = new MachineIDService();
            string? machineId = machineService.GetMachineId();

            //if (machineId != null)
            //{
            Console.WriteLine("Machine ID: " + machineId);
            var checkIn = new EmployeeAttendance
            {
                EmployeeId = emp.EmployeeId,
                CheckInTime = DateTime.Now,
                CheckinMadeSystemIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                CheckOutMadeSystemIP = null,
                CheckInPosition = CheckInPosition,
                CheckInMadeDateTime = DateTime.Now,
                CheckInMadeUserId = Convert.ToInt32(_userManager.GetUserId(User)),
            };

            _context.EmployeeAttendance.Add(checkIn);
            await _context.SaveChangesAsync();

            try
            {
                return Ok("Check-in successful.");
            }
            catch (Exception ex)
            {
                return Ok(ex.ToString());
            }
            //}
            //else
            //{
            //    return BadRequest("Machine ID file not found.");
            //}
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

            var timeSheet = _context.Timesheets
                            .Where(t => t.EmployeeId == emp.EmployeeId && t.Date.Date == today.Date)
                            .Any();
            if (!timeSheet)
            {
                return BadRequest("You have not update your timesheet. Please update and proceed.");
            }

            var machineService = new MachineIDService();
            string? machineId = machineService.GetMachineId();

            //if (machineId != null)
            //{
                checkInRecord.CheckOutTime = DateTime.Now;
                checkInRecord.CheckOutPosition = CheckOutPosition;
                checkInRecord.CheckOutMadeSystemIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                checkInRecord.CheckOutMadeDateTime = DateTime.Now;
                checkInRecord.CheckOutMadeUserId = Convert.ToInt32(_userManager.GetUserId(User));
                await _context.SaveChangesAsync();

                try
                {
                    return Ok("Check-out successful.");
                }
                catch (Exception ex)
                {
                    return Ok(ex.ToString());
                }
            //}
            //else
            //{
            //    return BadRequest("Machine ID file not found.");
            //}
        }

    }
}
