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

namespace SparkHRMS.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
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

            var attendances= results.Select(x => new EmployeeAttendanceDto
            {
                Id = x.Attendance?.Id ?? 0,
                EmployeeId = x.Employee.EmployeeId,
                EmployeeName = x.Employee.Name,
                CheckInDateTime = x.Attendance?.CheckInTime, // Nullable DateTime
                CheckOutDateTime = x.Attendance?.CheckOutTime, // Nullable DateTime
                IP = x.Attendance?.CheckinMadeSystemIP,
                CheckInPosition  = x.Attendance?.CheckInPosition,
                CheckOutPosition  = x.Attendance?.CheckOutPosition,
            }).ToList();

            foreach(var attendance in attendances)
            {
                attendance.WorkingHours = Utility.CalculateWorkingHours(attendance.CheckInDateTime, attendance.CheckOutDateTime);
                attendance.CheckInLocation = await Utility.GetLocationFromCoordinates(attendance.CheckInPosition);
                attendance.CheckOutLocation = await Utility.GetLocationFromCoordinates(attendance.CheckOutPosition);
            };

            ViewBag.SelectedDate = selectedDate;

            return View(attendances);
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
                Id = attendance == null ? 0 :attendance.Id ,
                EmployeeName = emp.Name,
                CheckInDateTime = attendance == null ? null : attendance.CheckInTime,
                CheckOutDateTime = attendance == null ? null : attendance.CheckOutTime,
                WorkingHours = attendance == null ? string.Empty :  attendance.CheckOutTime.HasValue
                              ? string.Format("{0:%h} hours {0:%m} mins", attendance.CheckOutTime.Value - attendance.CheckInTime)
                              : string.Empty
            };

            if(employeeAttendance.CheckInDateTime != null)
            {
                employeeAttendance.CheckInTime = TimeOnly.FromDateTime((DateTime)(attendance?.CheckInTime));
                employeeAttendance.CheckOutTime = TimeOnly.FromDateTime((DateTime)(attendance?.CheckOutTime));
            }
            return View(employeeAttendance);
        }
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeAttendance attendance)
        {
            var existingCheckIn = _context.EmployeeAttendance
                .FirstOrDefault(c => c.EmployeeId == attendance.EmployeeId && c.CheckInTime.Date == attendance.CheckInTime.Date);

            if (existingCheckIn != null)
            {
                existingCheckIn.CheckInTime = attendance.CheckInTime;
                existingCheckIn.CheckOutTime = attendance.CheckOutTime;
                existingCheckIn.CheckinMadeSystemIP = HttpContext.Connection.RemoteIpAddress?.ToString();
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
                    CheckinMadeSystemIP = HttpContext.Connection.RemoteIpAddress?.ToString()
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

    }
}
