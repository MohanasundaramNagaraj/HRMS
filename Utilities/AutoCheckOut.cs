
using Humanizer;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SparkHRMS.ViewModels;
using SparkHRMS.Interfaces;
using System.Net;
using System.Net.Mail;
using IAutoCheckOut = SparkHRMS.Interfaces.IAutoCheckOut;
using SparkHRMS.Data.Entities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SparkHRMS.Data;
using Microsoft.EntityFrameworkCore;
using Hangfire.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;
using Serilog;
using SparkHRMS.Services;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography.Xml;
using System.Reflection.Metadata;
namespace SparkHRMS.Utilities
{
    public class AutoCheckOut : IAutoCheckOut
    {
        private readonly ApplicationDbContext _context;
        private readonly SmtpSettings _smtpSettings;
        private IConfiguration Configuration { get; }
        private readonly ILogger<AutoCheckOut> _logger;

        public AutoCheckOut(ApplicationDbContext context, ILogger<AutoCheckOut> logger, IConfiguration configuration, IOptions<SmtpSettings> smtpSettings)
        {
            _context = context;
            Configuration = configuration;
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
        }
        public async Task AutoCheckOutAsync(string email, string subject, string htmlMessage)
        {
            string dateTimeForCheckinCheckoutSubject = "";
            if (subject == "CheckIn" || subject == "CheckOut")
            {
                dateTimeForCheckinCheckoutSubject = DateTime.Now.ToString("dd MMM yyyy");
            }

            try
            {
                var today = DateTime.Today;
                var yesterday = DateTime.Today.AddDays(-1);
                var employeeAttendance = (from emp in _context.EmployeeAttendance
                                          join employee in _context.Employees on emp.EmployeeId equals employee.EmployeeId
                                          where emp.CheckInTime != null &&
                                                emp.CheckInTime.Date == today || emp.CheckInTime.Date == yesterday
                                                && emp.CheckOutTime == null
                                          select new
                                          {
                                              EmployeeAttendance = emp,
                                              Employee = employee
                                          }).ToList();

                foreach (var emplyoee in employeeAttendance)
                {
                    var empAttendance = (from em in _context.EmployeeAttendance
                                         where em.EmployeeId == emplyoee.Employee.EmployeeId && em.CheckInTime.Date== today || em.CheckInTime.Date == yesterday
                                         select em).FirstOrDefault();
                    empAttendance.CheckOutTime = emplyoee.EmployeeAttendance.CheckInTime.AddHours(9); // Add 9 hours to CheckInTime
                    empAttendance.IsAutoCheckedOut = true;
                    empAttendance.CheckinMadeSystemIP = IPAddress.Loopback.ToString();
                    _context.SaveChanges();
                    var record = empAttendance;
                    htmlMessage += @"
<div style=""width: 100%; padding: 20px; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <div style=""max-width: 800px; margin: 0 auto; background: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);"">
        <div style=""background-color: #17a2b8; color: #ffffff; padding: 15px; border-top-left-radius: 8px; border-top-right-radius: 8px;"">
            <h2 style=""margin: 0;"">Employee Attendance for " + today.ToString("dd MMM yyyy") + @"</h2>
        </div>
        <div style=""padding: 20px;"">";


                    var checkInTime = record.CheckInTime != null ? ((DateTime)record.CheckInTime).ToString("hh:mm tt", CultureInfo.InvariantCulture) : "Not Checked In";
                    var checkOutTime = record.CheckOutTime != null ? ((DateTime)record.CheckOutTime).ToString("hh:mm tt", CultureInfo.InvariantCulture) : "Not Checked Out";
                    string workingHours = CalculateWorkingHours(record.CheckInTime, record.CheckOutTime);


                    var employeeName = emplyoee.Employee.Name;
                    var systemIP = emplyoee.EmployeeAttendance.CheckinMadeSystemIP ?? "N/A";
                    var checkOutIP = emplyoee.EmployeeAttendance.CheckOutMadeSystemIP ?? "N/A";
                    string siteUrl = Configuration["AppSettings:ThisSiteUrl"];
                    //htmlMessage += @"
                    //<p>Dear "+ employeeName +@",
                    //You has been Auto Check Out.Please Avoid this Kind of Process.
                    //</p>
                    //                            <div style=""border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 15px; padding: 15px; background-color: #f9f9f9;"">
                    //                                <h3 style=""margin: 0 0 10px;"">" + employeeName + @"</h3>
                    //                                <p style=""margin: 5px 0;""><strong>Check-In Time:</strong> " + checkInTime + @"</p>
                    //                                <p style=""margin: 5px 0;""><strong>Check-Out Time:</strong> " + checkOutTime + @"</p>
                    //                                <p style=""margin: 5px 0;""><strong>Working Hours:</strong> " + workingHours + @"</p>
                    //                                <p style=""margin: 5px 0;""><strong>Check-In IP:</strong> " + systemIP + @"</p>
                    //                                <p style=""margin: 5px 0;""><strong>Check-Out IP:</strong> " + checkOutIP + @"</p>
                    //                                <button style=""margin-top: 10px; padding: 10px 15px; background-color: #28a745; color: #ffffff; border: none; border-radius: 5px; cursor: pointer;"">
                    //                                    Auto Check-Out
                    //                                </button>
                    //                            </div>";
                    //                    htmlMessage += @"
                    //                            </div>
                    //                        </div>
                    //                    </div>";

                    //                    htmlMessage = $@"
                    //<p>Dear {employeeName},</p>
                    //<p>Our system has detected that you did not check out properly. As a result, the system has automatically checked you out. Please note that this may affect your timesheet working hours.</p>
                    //<p>We kindly request you to ensure proper check-out in the future to avoid any discrepancies.</p>

                    // <div style=""""border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 15px; padding: 15px; background-color: #f9f9f9;"""">
                    //                                                    <h3 style=""""margin: 0 0 10px;"""">"" + employeeName + @""</h3>
                    //                                                    <p style=""""margin: 5px 0;""""><strong>Check-In Time:</strong> "" + checkInTime + @""</p>
                    //                                                    <p style=""""margin: 5px 0;""""><strong>Check-Out Time:</strong> "" + checkOutTime + @""</p>
                    //                                                    <p style=""""margin: 5px 0;""""><strong>Working Hours:</strong> "" + workingHours + @""</p>
                    //                                                    <p style=""""margin: 5px 0;""""><strong>Check-In IP:</strong> "" + systemIP + @""</p>
                    //                                                    <p style=""""margin: 5px 0;""""><strong>Check-Out IP:</strong> "" + checkOutIP + @""</p>
                    //                                                    <button style=""""margin-top: 10px; padding: 10px 15px; background-color: #28a745; color: #ffffff; border: none; border-radius: 5px; cursor: pointer;"""">
                    //                                                        Auto Check-Out
                    //                                                    </button>
                    //                                                </div>"";
                    //                                        htmlMessage += @""
                    //                                                </div>
                    //                                            </div>
                    //                                        </div>"";

                    //<p>Regards,</p>
                    //<p>HR Team<br/>Spark IT Tech</p>
                    //<p>Please visit <a href='{siteUrl}' style='color: #007bff; text-decoration: none;'>{Configuration["AppSettings:SiteTitle"]}</a> for more information.</p>";

                    htmlMessage = $@"
<p>Dear {employeeName},</p>
<p>Our system has detected that you did not check out properly. As a result, the system has automatically checked you out. Please note that this may affect your timesheet working hours.</p>
<p>We kindly request you to ensure proper check-out in the future to avoid any discrepancies.</p>

<div style='border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 15px; padding: 15px; background-color: #f9f9f9;'>
    <h3 style='margin: 0 0 10px;'>{employeeName}</h3>
    <p style='margin: 5px 0;'><strong>Check-In Time:</strong> {checkInTime}</p>
    <p style='margin: 5px 0;'><strong>Check-Out Time:</strong> {checkOutTime}</p>
    <p style='margin: 5px 0;'><strong>Working Hours:</strong> {workingHours}</p>
    <p style='margin: 5px 0;'><strong>Check-In IP:</strong> {systemIP}</p>
    <button style='margin-top: 10px; padding: 10px 15px; background-color: #28a745; color: #ffffff; border: none; border-radius: 5px; cursor: pointer;'>
        Auto Check-Out
    </button>
</div>

<p>Regards,</p>
<p>HR Team<br/>Spark IT Tech</p>
<p>Please visit <a href='{siteUrl}' style='color: #007bff; text-decoration: none;'>{Configuration["AppSettings:SiteTitle"]}</a> for more information.</p>";



                    //htmlMessage += await getAutoCheckOutEmailContent(DateTime.Today);
                    subject = Configuration["EmailSenderSettings:Subject:AutoCheckOut"] + " on " + dateTimeForCheckinCheckoutSubject;

                    var apiKey = Configuration["EmailSenderSettings:SendGridAPIKey"];
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress(Configuration["EmailSenderSettings:From"], Configuration["EmailSenderSettings:UserName"]);
                    //var mailTo = (from emp in _context.Users);
                    var to = new EmailAddress(emplyoee.Employee.Email);
                    // var to = new EmailAddress(email, email);
                    var plainTextContent = "";
                    var htmlContent = htmlMessage;

                    var emailLogs = new EmailLogs
                    {
                        Recipient = email,
                        Cc = string.Empty,
                        Subject = subject,
                        Body = htmlMessage,
                        SentDate = DateTime.Now,
                        IsSuccessful = false,
                        ErrorMessage = string.Empty
                    };

                    _context.EmailLogs.Add(emailLogs);
                    _context.SaveChanges();

                    int logId = emailLogs.Id;
                    try
                    {
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = await client.SendEmailAsync(msg);
                        await Task.Delay(500); // Delay in milliseconds

                        var log = _context.EmailLogs.Where(x => x.Id == logId).FirstOrDefault();
                        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                        {

                            log.IsSuccessful = true;
                        }
                        else
                        {
                            string responseBody = await response.Body.ReadAsStringAsync();
                            log.IsSuccessful = false;
                            log.ErrorMessage = responseBody;
                        }

                        _context.Entry(log).State = EntityState.Modified;
                        _context.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        var log = _context.EmailLogs.Where(x => x.Id == logId).FirstOrDefault();

                        log.IsSuccessful = false;
                        log.ErrorMessage = ex.ToString();
                        _context.Entry(log).State = EntityState.Modified;
                        _context.SaveChanges();
                        // log an error message or throw an exception or both.
                        _logger.LogError(ex.Message);
                        throw;
                    }
                }

                var autoCheckOutEmployees = (from emp in _context.EmployeeAttendance
                                             join employee in _context.Employees on emp.EmployeeId equals employee.EmployeeId
                                             where (
                                             //emp.CheckInTime.Date == today ||
                                             emp.CheckInTime.Date == yesterday)
                                                   && emp.IsAutoCheckedOut == true
                                             select new
                                             {
                                                 EmployeeAttendance = emp,
                                                 Employee = employee
                                             }).ToList();

                 htmlMessage = @"
<div style=""width: 100%; padding: 20px; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <div style=""max-width: 800px; margin: 0 auto; background: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);"">
        <div style=""background-color: #17a2b8; color: #ffffff; padding: 15px; border-top-left-radius: 8px; border-top-right-radius: 8px;"">
            <h2 style=""margin: 0;"">Employee Attendance for " + today.ToString("dd MMM yyyy") + @"</h2>
        </div>
        <div style=""padding: 20px;"">
            <p>Dear Team,</p>
            <p>Below are the details of employees who were automatically checked out by the system:</p>";

                foreach (var employee in autoCheckOutEmployees)
                {
                    var record = employee.EmployeeAttendance;

                    var checkInTime = record.CheckInTime != null ? ((DateTime)record.CheckInTime).ToString("hh:mm tt", CultureInfo.InvariantCulture) : "Not Checked In";
                    var checkOutTime = record.CheckOutTime != null ? ((DateTime)record.CheckOutTime).ToString("hh:mm tt", CultureInfo.InvariantCulture) : "Not Checked Out";
                    string workingHours = CalculateWorkingHours(record.CheckInTime, record.CheckOutTime);

                    var employeeName = employee.Employee.Name;
                    var systemIP = record.CheckinMadeSystemIP ?? "N/A";
                    var checkOutIP = record.CheckOutMadeSystemIP ?? "N/A";

                    htmlMessage += $@"
    <div style='border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 15px; padding: 15px; background-color: #f9f9f9;'>
        <h3 style='margin: 0 0 10px;'>{employeeName}</h3>
        <p style='margin: 5px 0;'><strong>Check-In Time:</strong> {checkInTime}</p>
        <p style='margin: 5px 0;'><strong>Check-Out Time:</strong> {checkOutTime}</p>
        <p style='margin: 5px 0;'><strong>Working Hours:</strong> {workingHours}</p>
        <p style='margin: 5px 0;'><strong>Check-In IP:</strong> {systemIP}</p>
        <p style='margin: 5px 0;'><strong>Check-Out IP:</strong> {checkOutIP}</p>
    </div>";
                }

                htmlMessage += @"
            <p>Regards,</p>
            <p>HR Team<br/>Spark IT Tech</p>
            <p>Please visit <a href='" + Configuration["AppSettings:ThisSiteUrl"] + @"' style='color: #007bff; text-decoration: none;'>" + Configuration["AppSettings:SiteTitle"] + @"</a> for more information.</p>
        </div>
    </div>
</div>";

                subject = Configuration["EmailSenderSettings:Subject:AutoCheckOut"] + " on " + dateTimeForCheckinCheckoutSubject;

                // Send the email using SendGrid
                try
                {
                    var apiKey = Configuration["EmailSenderSettings:SendGridAPIKey"];
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress(Configuration["EmailSenderSettings:From"], Configuration["EmailSenderSettings:UserName"]);
                    var to = new EmailAddress("nandhakumarmurugesan45@gmail.com");
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
                    var response = await client.SendEmailAsync(msg);
                    await Task.Delay(500); // Delay in milliseconds
                    var emailLogs = new EmailLogs
                    {
                        Recipient = email,
                        Cc = string.Empty,
                        Subject = subject,
                        Body = htmlMessage,
                        SentDate = DateTime.Now,
                        IsSuccessful = false,
                        ErrorMessage = string.Empty
                    };

                    _context.EmailLogs.Add(emailLogs);
                    _context.SaveChanges();
                    int logId = emailLogs.Id;
                    var log = _context.EmailLogs.Where(x => x.Id == logId).FirstOrDefault();
                    if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                    {

                        log.IsSuccessful = true;
                    }
                    else
                    {
                        string responseBody = await response.Body.ReadAsStringAsync();
                        log.IsSuccessful = false;
                        log.ErrorMessage = responseBody;
                    }

                    _context.Entry(log).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Log error
                    _logger.LogError(ex.Message);
                    throw;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
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
    }
}
