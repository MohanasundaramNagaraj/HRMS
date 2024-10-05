using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using SparkHRMS.Interfaces;
using SparkHRMS.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SparkHRMS.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using System.Globalization;
using Hangfire;

namespace SparkHRMS.Services
{
    public class EmailQueueManager
    {
        private readonly IConfiguration Configuration;
        private readonly IEmailSender _emailService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public EmailQueueManager(IEmailSender emailService, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBackgroundJobClient backgroundJobClient)
        {
            _emailService = emailService;
            _context = context;
            _userManager = userManager;
            _backgroundJobClient = backgroundJobClient;
        }
        public async Task SendScheduledEmail(string Event)
        {
            var selectedDate = DateTime.Today;

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

            var results = await query
                .OrderBy(x => x.Attendance.CheckInTime)
                .ToListAsync();

            var attendanceDtos = results.Select(x => new EmployeeAttendanceDto
            {
                Id = x.Attendance?.Id ?? 0,
                EmployeeName = x.Employee.Name,
                CheckInDateTime = x.Attendance?.CheckInTime, // Nullable DateTime
                CheckOutDateTime = x.Attendance?.CheckOutTime, // Nullable DateTime
                WorkingHours = CalculateWorkingHours(x.Attendance?.CheckInTime, x.Attendance?.CheckOutTime),
                IP = x.Attendance?.CheckinMadeSystemIP,
                CheckOutMadeSystemIP = x.Attendance?.CheckOutMadeSystemIP
            }).ToList();


            string html = @"
    <div style=""width: 100%; padding: 20px; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
        <div style=""max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);"">
            <div style=""background-color: #17a2b8; color: #ffffff; padding: 15px; border-top-left-radius: 8px; border-top-right-radius: 8px;"">
                <h2 style=""margin: 0;"">Employee Attendance for " + selectedDate.ToString("dd MMM yyyy") + @"</h2>
            </div>
            <div style=""padding: 20px;"">
                <table style=""width: 100%; border-collapse: collapse;"">
                    <thead style=""background-color: #343a40; color: #ffffff;"">
                        <tr>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Employee Name</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Check-In Time</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Check-Out Time</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Working Hours</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Check In Made IP</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Check Out Made IP</th>
                        </tr>
                    </thead>
                    <tbody>";

            foreach (var record in attendanceDtos)
            {
                var checkInTime = record.CheckInDateTime != null ? ((DateTime)record.CheckInDateTime).ToString("hh:mm tt", CultureInfo.InvariantCulture) : "";
                var checkOutTime = record.CheckOutDateTime != null ? ((DateTime)record.CheckOutDateTime).ToString("hh:mm tt", CultureInfo.InvariantCulture) : "";
                var workingHours = record.WorkingHours ?? "";
                var employeeName = record.EmployeeName ?? "";
                var systemIP = record.IP ?? "";
                var checkOutIP = record.CheckOutMadeSystemIP ?? "";
                html += @"
        <tr>
            <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + employeeName + @"</td>
            <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + checkInTime + @"</td>
            <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + checkOutTime + @"</td>
            <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + workingHours + @"</td>
            <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + systemIP + @"</td>
            <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + checkOutIP + @"</td>
        </tr>";
            }

            html += @"
                    </tbody>
                </table>
            </div>
        </div>
    </div>";

            var adminUsers = await GetAdminsAndSuperAdminsAsync();
            foreach (var user in adminUsers)
            {
                _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync(user.Email, Event, html));
            }
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

        public async Task<List<ApplicationUser>> GetAdminsAndSuperAdminsAsync()
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            // Combine the lists (remove duplicates if necessary)
            var result = superAdmins.Union(admins).ToList();

            return result;
        }
    }
}
