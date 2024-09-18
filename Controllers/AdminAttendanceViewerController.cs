using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using SparkHRMS.ViewModels;

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
        public async Task<IActionResult> Index(DateTime? date = null, string employeeName = null)
        {
            var selectedDate = date ?? DateTime.Today;

            var query = from e in _context.Employees
                        join a in _context.EmployeeAttendance
                        on e.EmployeeId equals a.EmployeeId into attendanceGroup
                        from a in attendanceGroup
                        .Where(a => a.CheckInTime.Date == selectedDate)
                        .DefaultIfEmpty()
                        select new
                        {
                            Employee = e,
                            Attendance = a,
                        };

            if (!string.IsNullOrEmpty(employeeName))
            {
                query = query.Where(x => x.Employee.Name.Contains(employeeName));
            }

            var results = await query
                .OrderBy(x => x.Attendance.CheckInTime)
                .ToListAsync();

            var attendanceDtos = results.Select(x => new EmployeeAttendanceDto
            {
                Id = x.Attendance?.Id ?? 0,
                EmployeeName = x.Employee.Name,
                CheckInTime = x.Attendance?.CheckInTime, // Nullable DateTime
                CheckOutTime = x.Attendance?.CheckOutTime, // Nullable DateTime
                WorkingHours = CalculateWorkingHours(x.Attendance?.CheckInTime, x.Attendance?.CheckOutTime),
                IP = x.Attendance?.CheckinMadeSystemIP
            }).ToList();

            ViewBag.SelectedDate = selectedDate;
            ViewBag.EmployeeName = employeeName; // For retaining filter value

            return View(attendanceDtos);
        }
        private string CalculateWorkingHours(DateTime? checkInTime, DateTime? checkOutTime)
        {
            if (checkInTime.HasValue && checkOutTime.HasValue)
            {
                var duration = checkOutTime.Value - checkInTime.Value;
                return string.Format("{0:%h} hours {0:%m} mins", duration);
            }
            return "N/A";
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
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
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

            attendance.CheckInTime = (DateTime)dto.CheckInTime;
            attendance.CheckOutTime = dto.CheckOutTime;

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
