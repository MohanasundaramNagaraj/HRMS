using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using SparkHRMS.ViewModels;
using SparkHRMS.Utilities;
using Humanizer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace SparkHRMS.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminAttendanceViewerController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly Utility _utilityService;

       
        public AdminAttendanceViewerController(Utility utilityService, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _utilityService = utilityService;
        }
        public async Task<IActionResult> Index(DateTime? date = null)
        {
            var selectedDate = date ?? DateTime.Today;

            var query = from e in _context.Employees
                        join user in _context.Users on e.ApplicationUserId equals user.Id
                        join a in _context.EmployeeAttendance
                        on e.EmployeeId equals a.EmployeeId into attendanceGroup
                        from a in attendanceGroup
                        .Where(a => a.CheckInTime.Date == selectedDate && user.IsActive)
                        .DefaultIfEmpty()
                        select new
                        {
                            Employee = e,
                            Attendance = a,
                        };

            var results = await query
                .OrderBy(x => x.Attendance.CheckInTime)
                .ToListAsync();

            var attendances = results.Select(x => new EmployeeAttendanceDto
            {
                Id = x.Attendance?.Id ?? 0,
                EmployeeId = x.Employee.EmployeeId,
                EmployeeName = x.Employee.Name,
                CheckInDateTime = x.Attendance?.CheckInTime, // Nullable DateTime
                CheckOutDateTime = x.Attendance?.CheckOutTime, // Nullable DateTime
                IP = x.Attendance?.CheckinMadeSystemIP,
                CheckOutMadeSystemIP = x.Attendance?.CheckOutMadeSystemIP,
                CheckInPosition = x.Attendance?.CheckInPosition,
                CheckOutPosition = x.Attendance?.CheckOutPosition,
            }).ToList();

            foreach (var attendance in attendances)
            {
                attendance.WorkingHours = _utilityService.CalculateWorkingHours(attendance.CheckInDateTime, attendance.CheckOutDateTime);
                attendance.CheckInLocation = await _utilityService.GetLocationFromCoordinates(attendance.CheckInPosition);
                attendance.CheckOutLocation = await _utilityService.GetLocationFromCoordinates(attendance.CheckOutPosition);
            };

            ViewBag.SelectedDate = selectedDate;

            return View(attendances);
        }

        public async Task<IActionResult> EmployeeMonthlySummary(int? Month)
        {
            var today = DateTime.Today;
            var year = today.Year;

            var startOfMonth = Month.HasValue ? new DateTime(year, Month.Value, 1) : new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Get the last day of the month

            List<EmployeeMonthlySummaryWithStatusCount> EmployeeMonthlySummaryWithStatusCounts = new List<EmployeeMonthlySummaryWithStatusCount>();

            foreach (var emp in _context.Employees.ToList())
            {
                var attendanceRecords = await _context.EmployeeAttendance
                      .Where(e => e.EmployeeId == emp.EmployeeId && e.CheckInTime.Date >= startOfMonth && e.CheckInTime.Date <= endOfMonth)
                      .OrderBy(e => e.CheckInTime)
                      .ToListAsync();

                var dailyAttendanceRecords = new List<EmployeeAttendanceDto>();
                TimeSpan totalWorkingTime = TimeSpan.Zero;

                var allDatesInMonth = Enumerable.Range(0, DateTime.DaysInMonth(today.Year, Month.HasValue ? Month.Value : today.Month))
                                       .Select(day => new DateTime(today.Year, Month.HasValue ? Month.Value : today.Month, day + 1))
                                       .ToList();

                // Fetch holidays from the database or your holidays table
                var holidays = await _context.Holiday
                                    .Where(h => h.Date >= startOfMonth && h.Date <= endOfMonth)
                                    .Select(h => h.Date.Date)
                                    .ToListAsync();

                var statuses = new List<AttendanceStatus>();
                foreach (var date in allDatesInMonth)
                {
                    var attendanceRecord = attendanceRecords.FirstOrDefault(a => a.CheckInTime.Date == date);
                    TimeSpan? workingTime = null;

                    AttendanceStatus status = AttendanceStatus.Absent;
                    if (holidays.Contains(date))
                    {
                        status = AttendanceStatus.Holiday;
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

                    statuses.Add(status);
                }

                EmployeeMonthlySummaryWithStatusCount EmployeeMonthlySummaryWithStatusCount = new EmployeeMonthlySummaryWithStatusCount();

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
                    EmpCode = emp.EmployeeCode,
                    ReportingHeadMailID = emp.ReportingHeadMailID,
                };

                EmployeeAttendanceStatusCounts EmployeeAttendanceStatusCounts = new EmployeeAttendanceStatusCounts();

                EmployeeAttendanceStatusCounts.EmployeePresentCountOnMonth = statuses.Where(x => x == AttendanceStatus.Present).Count();
                EmployeeAttendanceStatusCounts.EmployeeHalfDayCountOnMonth = statuses.Where(x => x == AttendanceStatus.HalfDay).Count();
                EmployeeAttendanceStatusCounts.EmployeeAbsentCountOnMonth = statuses.Where(x => x == AttendanceStatus.Absent).Count();
                EmployeeAttendanceStatusCounts.EmployeeHolidayCountOnMonth = statuses.Where(x => x == AttendanceStatus.Holiday).Count();
                EmployeeAttendanceStatusCounts.EmployeeWeekendCountOnMonth = statuses.Where(x => x == AttendanceStatus.Weekend).Count();
                EmployeeAttendanceStatusCounts.EmployeePermissionNeededCountOnMonth = statuses.Where(x => x == AttendanceStatus.PermissionNeeded).Count();
                EmployeeAttendanceStatusCounts.EmployeePendingCheckOutNeededCountOnMonth = statuses.Where(x => x == AttendanceStatus.PendingCheckOut).Count();

                EmployeeMonthlySummaryWithStatusCount.EmployeeDetails = employeeDetails;
                EmployeeMonthlySummaryWithStatusCount.EmployeeAttendanceStatusCounts = EmployeeAttendanceStatusCounts;

                EmployeeMonthlySummaryWithStatusCounts.Add(EmployeeMonthlySummaryWithStatusCount);
            }


            ViewBag.SelectedYear = year;
            ViewBag.SelectedMonth = Month;

            return View(EmployeeMonthlySummaryWithStatusCounts);
        }

        [HttpGet]
        public IActionResult Add(int EmployeeID, DateTime Date)
        {
            ViewBag.EmployeeID = EmployeeID;
            ViewBag.Date = Date;
            var emp = _context.Employees.Where(x => x.EmployeeId == EmployeeID).FirstOrDefault();
            var attendance = _context.EmployeeAttendance
                .Include(e => e.Employee)
                .SingleOrDefault(e => e.EmployeeId == EmployeeID && e.CheckInTime.Date == Date.Date);

            var employeeAttendance = new EmployeeAttendanceDto
            {
                Id = attendance == null ? 0 : attendance.Id,
                EmployeeName = emp.Name,
                CheckInDateTime = attendance == null ? null : attendance.CheckInTime,
                CheckOutDateTime = attendance == null ? null : attendance.CheckOutTime,
                WorkingHours = attendance == null ? string.Empty : attendance.CheckOutTime.HasValue
                              ? string.Format("{0:%h} hours {0:%m} mins", attendance.CheckOutTime.Value - attendance.CheckInTime)
                              : string.Empty
            };

            if (attendance?.CheckInTime != null || employeeAttendance.CheckInDateTime != null)
            {
                employeeAttendance.CheckInTime = TimeOnly.FromDateTime((DateTime)(attendance?.CheckInTime));
            }
            if (attendance?.CheckOutTime != null)
            {
                employeeAttendance.CheckOutTime = TimeOnly.FromDateTime((DateTime)(attendance?.CheckOutTime));
            }

            return View(employeeAttendance);
        }
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeAttendance attendance)
        {
            var existingCheckIn = _context.EmployeeAttendance
                .FirstOrDefault(c => c.EmployeeId == attendance.EmployeeId && c.CheckInTime.Date == attendance.CheckInTime.Date);

            var checkoutDateTime = Convert.ToDateTime(attendance.CheckOutTime);
            bool isMidnight = checkoutDateTime.Hour == 0 && checkoutDateTime.Minute == 0 && checkoutDateTime.Second == 0;
            if (isMidnight)
            {
                attendance.CheckOutTime = null;
            }

            var checkInDateTime = Convert.ToDateTime(attendance.CheckInTime);
            isMidnight = checkInDateTime.Hour == 0 && checkInDateTime.Minute == 0 && checkInDateTime.Second == 0;
            if (isMidnight)
            {
                return BadRequest("Please enter the Check In Details");
            }
            if (existingCheckIn != null)
            {
                existingCheckIn.CheckInTime = attendance.CheckInTime;
                existingCheckIn.CheckOutTime = attendance.CheckOutTime;
                existingCheckIn.CheckinMadeSystemIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                existingCheckIn.CheckOutMadeSystemIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                _context.Update(existingCheckIn);
                // return BadRequest("The Employee Already checked in for selected Date.");
            }
            else
            {
                var checkIn = new EmployeeAttendance
                {
                    EmployeeId = attendance.EmployeeId,
                    CheckInTime = attendance.CheckInTime,
                    CheckOutTime = attendance.CheckOutTime,
                    CheckinMadeSystemIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    CheckOutMadeSystemIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                };

                _context.EmployeeAttendance.Add(checkIn);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        public IActionResult Edit(int id)
        {

            // Retrieve the attendance record by Id and pass it to the view
            var attendance = _context.EmployeeAttendance
                .Include(e => e.Employee)
                .SingleOrDefault(e => e.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            var dto = new EmployeeAttendanceDto
            {
                Id = attendance.Id,
                EmployeeName = attendance.Employee.Name,
                CheckInDateTime = attendance.CheckInTime,
                CheckOutDateTime = attendance.CheckOutTime,
                WorkingHours = attendance.CheckOutTime.HasValue
                               ? string.Format("{0:%h} hours {0:%m} mins", attendance.CheckOutTime.Value - attendance.CheckInTime)
                               : "N/A"
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeAttendanceDto dto)
        {
            var attendance = await _context.EmployeeAttendance.FindAsync(dto.Id);

            if (attendance == null)
            {
                return NotFound();
            }

            attendance.CheckInTime = (DateTime)dto.CheckInDateTime;
            attendance.CheckOutTime = dto.CheckOutDateTime;

            _context.Update(attendance);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { date = ViewBag.SelectedDate, employeeName = ViewBag.EmployeeName });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var attendance = await _context.EmployeeAttendance.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            _context.EmployeeAttendance.Remove(attendance);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { date = ViewBag.SelectedDate, employeeName = ViewBag.EmployeeName });
        }

        public async Task<ActionResult> NewRequest()
        {
            try
            {
                EmployeeAttendanceRequest request = new EmployeeAttendanceRequest();

                request.EmployeeId = 1;
                request.RequestType = "Leave";
                request.StartDate = DateTime.Now;
                request.EndDate = DateTime.Now.AddDays(1);
                request.Reason = "";

                // request.StartTime = new TimeSpan(9, 0, 0); 
                // request.EndTime = new TimeSpan(5, 0, 0); 

                request.Status = "Pending";
                request.SubStatusId = 1;
                request.CreatedBy = 1;//User.Identity.Name;
                request.CreatedTime = DateTime.Now;

                await _context.EmployeeAttendanceRequest.AddAsync(request);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ActionResult> UpdateRequest(int RequestId)
        {
            try
            {
                var request = _context.EmployeeAttendanceRequest.Where(x => x.Id == RequestId).FirstOrDefault();

                if (request == null)
                {

                    _context.Update(request).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                   
                }

                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> EmployeeRequests(int? EmployeeID, int? Month, int? Year)
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

            var today = DateTime.Today;
            var year = Year.HasValue ? (int)Year : today.Year;

            var startOfMonth = Month.HasValue ? new DateTime(year, Month.Value, 1) : new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Get the last day of the month

            ViewBag.EmployeeList = _context.Employees.ToList();
            ViewBag.SelectedEmployeeID = employeeDetails.EmployeeId;
            ViewBag.SelectedMonth = startOfMonth.Month;
            ViewBag.SelectedYear = startOfMonth.Year;

            List<EmployeeRequests> employeeRequestsList = new List<EmployeeRequests>();
            var query = (from r in _context.EmployeeAttendanceRequest
                         join e in _context.Employees on r.EmployeeId equals e.EmployeeId
                         select new
                         {
                             EmployeeID = e.EmployeeId,
                             EmpCode = e.EmployeeCode,
                             Name = e.Name,
                             RequestType = r.RequestType,
                             StartDate = r.StartDate,
                             EndDate = r.EndDate,
                             //RequestDateTime = r.StartDate + " - " + r.EndDate,
                             Reason = r.Reason,
                             ApprovalStatus = r.Status
                         });

            if (EmployeeID != null)
            {
                query = from q in query
                        where q.EmployeeID == EmployeeID
                        select q;
            }

            if (Month != null)
            {
                query = from q in query
                        where q.StartDate.Month == Month
                        select q;
            }

            if (Year != null)
            {
                query = from q in query
                        where q.StartDate.Year == Year
                        select q;
            }

            var res = (from q in query
                       select new EmployeeRequests
                       {
                           EmployeeID = q.EmployeeID,
                           EmpCode = q.EmpCode,
                           Name = q.Name,
                           RequestType = q.RequestType,
                           RequestDateTime = q.StartDate + " - " + q.EndDate,
                           Reason = q.Reason,
                           ApprovalStatus = q.ApprovalStatus
                       }).ToList();

            employeeRequestsList.AddRange(res);

            return View(employeeRequestsList);
        }
    }
}
