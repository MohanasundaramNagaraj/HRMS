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
using System.Collections.Generic;
using SendGrid.Helpers.Mail;
using SparkHRMS.Utilities;


namespace SparkHRMS.Services
{
    public class EmailQueueManager
    {
        private readonly IConfiguration Configuration;
        private readonly IEmailSender _emailService;
        private readonly IAutoCheckOut _autoCheckOut;
        private readonly IAutoTimeSheetMail _autoTimeSheetMail;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly Utility _utilityService;
        public EmailQueueManager(Utility utilityService, IEmailSender emailService,IAutoCheckOut autoCheckOut, IAutoTimeSheetMail autoTimeSheetMail, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBackgroundJobClient backgroundJobClient)
        {
            _emailService = emailService;
            _context = context;
            _userManager = userManager;
            _backgroundJobClient = backgroundJobClient;
            _autoCheckOut = autoCheckOut;
            _autoTimeSheetMail = autoTimeSheetMail;
            _utilityService = utilityService;
        }
        public async Task SendScheduledEmail(string Event)
        {
            string html = "";
            DateTime Yesterday = DateTime.Today.Date.AddDays(-1);
            html += await getEmailContent(Yesterday);
            html += await getEmailContent(DateTime.Today);

            var adminUsers = await GetAdminsAndSuperAdminsAsync();
            foreach (var user in adminUsers)
            {
                await Task.Delay(5000);
                _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync(user.Email, Event, html));
            }
        }
        public async Task AutoCheckOutEmail(string Event)
        {
            string html = "";
            //DateTime Yesterday = DateTime.Today.Date.AddDays(-1);

            var adminUsers = await GetAdminsAndSuperAdminsAsync();
            foreach (var user in adminUsers)
            {
                _backgroundJobClient.Enqueue(() => _autoCheckOut.AutoCheckOutAsync(user.Email, Event, html));
            }
        }        
        public async Task AutoTimeSheetEmail(string Event)
        {
            string html = "";
            List<ApplicationUser> users = (await _userManager.GetUsersInRoleAsync("Employee")).ToList();
            List<ApplicationUser> admins = await GetAdminsAndSuperAdminsAsync();
            List<EmailAddress> ccs = new List<EmailAddress>();
            foreach(var i in admins)
            {
                EmailAddress cc = new EmailAddress();
                cc.Email = i.Email;
                cc.Name = i.UserName;
                ccs.Add(cc);
            }

            List<Employee> employees = users
            .Select(u =>
            {
                var employee = _context.Employees.Where(v=>v.IsActive == true).FirstOrDefault(x => x.ApplicationUserId == u.Id);
                if (employee != null)
                {
                    return new Employee
                    {
                        EmployeeId = employee.EmployeeId,
                        Name = employee.Name,
                        Email = u.Email,
                        Designation =employee.Designation
                    };
                }

                return null; // Return null when no match is found
            })
            .Where(e => e != null) // Filter out nulls
            .ToList();


            //_backgroundJobClient.Enqueue(() => _autoTimeSheetMail.AutoTimeSheetMailAsync(employees, Event, html, ccs));
            
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

        private async Task<string> getEmailContent(DateTime selectedDate)
        {
           
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
                //Id = x.Attendance?.Id ?? 0,
                //EmployeeName = x.Employee.Name,
                //CheckInDateTime = x.Attendance?.CheckInTime, // Nullable DateTime
                //CheckOutDateTime = x.Attendance?.CheckOutTime, // Nullable DateTime
                WorkingHours = CalculateWorkingHours(x.Attendance?.CheckInTime, x.Attendance?.CheckOutTime),
                //IP = x.Attendance?.CheckinMadeSystemIP,
                //CheckOutMadeSystemIP = x.Attendance?.CheckOutMadeSystemIP

                Id = x.Attendance?.Id ?? 0,
                EmployeeId = x.Employee.EmployeeId,
                EmployeeName = x.Employee.Name,
                CheckInDateTime = x.Attendance?.CheckInTime, // Nullable DateTime
                CheckOutDateTime = x.Attendance?.CheckOutTime, // Nullable DateTime
                IP = x.Attendance?.CheckinMadeSystemIP,
                CheckOutMadeSystemIP = x.Attendance?.CheckOutMadeSystemIP,
                CheckInPosition = x.Attendance?.CheckInPosition,
                CheckOutPosition = x.Attendance?.CheckOutPosition,

                IsPermission = x.Attendance?.IsPermission ?? false,
                PermissionStartTime = x.Attendance?.PermissionStartTime,
                PermissionEndTime = x.Attendance?.PermissionEndTime,

                IsOnDuty = x.Attendance?.IsOnDuty ?? false,
                DutyStartTime = x.Attendance?.DutyStartTime,
                DutyEndTime = x.Attendance?.DutyEndTime,

                IsLeave = x.Attendance?.IsLeave ?? false,
                IsHalfDayLeave = x.Attendance?.IsHalfDayLeave ?? false,

                CheckInMadeBy = x.Attendance != null ? _utilityService.GetUserNameById(x.Attendance.CheckInMadeUserId) : string.Empty,
                CheckOutMadeBy = x.Attendance != null ? _utilityService.GetUserNameById(x.Attendance.CheckOutMadeUserId) : string.Empty,

                CheckInMadeDateTime = x.Attendance?.CheckInMadeDateTime,
                CheckOutMadeDateTime = x.Attendance?.CheckOutMadeDateTime,
            }).ToList();


            string html = @"
    <div style=""width: 100%; padding: 20px; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
        <div style="" margin: 0 auto; background: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);"">
            <div style=""background-color: #17a2b8; color: #ffffff; padding: 15px; border-top-left-radius: 8px; border-top-right-radius: 8px;"">
                <h2 style=""margin: 0;"">Employee Attendance for " + selectedDate.ToString("dd MMM yyyy") + @"</h2>
            </div>
            <div style=""padding: 20px;overflow: scroll;"">
                <table style=""overflow: scroll; border-collapse: collapse;"">
                    <thead style=""background-color: #343a40; color: #ffffff;"">
                        <tr>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Employee Name</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Permission/Leave/On Duty Details</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Check-In Time</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Check-Out Time</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Working Hours</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Check In Details</th>
                            <th style=""padding: 10px; border: 1px solid #dee2e6; text-align: left;"">Check Out Details</th>
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


                string displayText = string.Empty;
                string displayTextColor = string.Empty;
                string onDutyDisplayText = string.Empty;
                string onDutyDisplayTextColor = string.Empty;

                if (record.IsLeave || record.IsPermission || record.IsOnDuty || record.IsHalfDayLeave)
                {
                    if (record.IsLeave)
                    {
                        displayText = "\n Leave";
                    }

                    if (!record.IsLeave && record.IsHalfDayLeave
                    //&& record.HalfDayLeave.HasValue
                    )
                    {
                        displayText += $"\n Half Day Leave";
                    }

                    if (!record.IsLeave && record.IsPermission)
                    {
                        DateTime? permissionStart = record?.PermissionStartTime as DateTime?;
                        DateTime? permissionEnd = record?.PermissionEndTime as DateTime?;

                        var permissionTime = (permissionStart.HasValue && permissionStart.Value.TimeOfDay != TimeSpan.Zero)
                        ? permissionStart.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                        : "";

                        var permissionEndTime = (permissionEnd.HasValue && permissionEnd.Value.TimeOfDay != TimeSpan.Zero)
                        ? permissionEnd.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                        : "";

                        displayText += "\n Permission on " + $"{permissionTime} - {permissionEndTime}";
                    }
                    displayTextColor = "red";

                    if (!record.IsLeave && record.IsOnDuty)
                    {
                        DateTime? Start = record?.DutyStartTime as DateTime?;
                        DateTime? End = record?.DutyEndTime as DateTime?;

                        var StartTime = (Start.HasValue && Start.Value.TimeOfDay != TimeSpan.Zero)
                        ? Start.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                        : "";

                        var EndTime = (End.HasValue && End.Value.TimeOfDay != TimeSpan.Zero)
                        ? End.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                        : "";

                        onDutyDisplayText += "\n On Duty on " + $"{StartTime} - {EndTime}";
                        onDutyDisplayTextColor = "black";
                    }


                }

                DateTime? checkIn = record?.CheckInDateTime as DateTime?;
                DateTime? checkOut = record?.CheckOutDateTime as DateTime?;

               
                var checkIndisplayText = $"{checkInTime} - {checkOutTime}";

                var checkInMadeDateTime = record?.CheckInMadeDateTime as DateTime?;
                var checkInMadeTime = (checkInMadeDateTime.HasValue && checkInMadeDateTime.Value.TimeOfDay != TimeSpan.Zero)
                ? checkInMadeDateTime.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                : "";

                var checkOutMadeDateTime = record?.CheckOutMadeDateTime as DateTime?;
                var checkOutMadeTime = (checkOutMadeDateTime.HasValue && checkOutMadeDateTime.Value.TimeOfDay != TimeSpan.Zero)
                ? checkOutMadeDateTime.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                : "";


                string _html = "";

                if (record != null && (!string.IsNullOrEmpty(checkInMadeTime) || !string.IsNullOrEmpty(checkOutMadeTime)))
                {
                    _html +=
                    @"
                      <td style=""padding: 10px; border: 1px solid #dee2e6;"">
                            <span>IP: <b>" + record.IP + @"</b><br /></span>
                            <span>Made By: <b>" + record.CheckInMadeBy + @"</b><br /></span>
                            <span>Time: <b>" + checkInMadeTime + @"</b><br /></span>
                      </td>
                      <td style=""padding: 10px; border: 1px solid #dee2e6;"">
                            <span>IP: <b>" + record.IP + @"</b><br /></span>
                            <span>Made By: <b>" + record.CheckOutMadeBy + @"</b><br /></span>
                            <span>Time: <b>" + checkOutMadeTime + @"</b><br /></span>
                      </td>";
                }
                else
                {
                    _html += @"<td colspan=""2"" style=""padding:10px; border:1px solid #dee2e6; text-align:center;"">No Check-In / Check-Out</td>";
                }


                html += @"
                <tr>
                    <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + employeeName + @"</td>
                    <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + displayText + @"<br />" + onDutyDisplayText + @"</td>
                    <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + checkInTime + @"</td>
                    <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + checkOutTime + @"</td>
                    <td style=""padding: 10px; border: 1px solid #dee2e6;"">" + workingHours + @"</td>
                   "+ _html + @"</tr>";
            }

            html += @"
                    </tbody>
                </table>
            </div>
        </div>
    </div>";
            return html;
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
