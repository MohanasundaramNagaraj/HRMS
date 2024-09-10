using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using System;
using System.Linq;
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
        public IActionResult Index()
        {
            return View();
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
