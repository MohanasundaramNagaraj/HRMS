
using Hangfire.Logging;
using Humanizer;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Interfaces;
using SparkHRMS.Services;
using SparkHRMS.ViewModels;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Security.Cryptography.Xml;
using static MailKit.Telemetry;
using static System.Net.Mime.MediaTypeNames;
using IAutoCheckOut = SparkHRMS.Interfaces.IAutoCheckOut;
namespace SparkHRMS.Utilities
{
    public class AutoCheckOut : IAutoCheckOut
    {
        private readonly ApplicationDbContext _context;
        private readonly SmtpSettings _smtpSettings;
        private IConfiguration Configuration { get; }
        private readonly ILogger<AutoCheckOut> _logger;
        private readonly Interfaces.IEmailSender _emailService;
        public AutoCheckOut(ApplicationDbContext context, Interfaces.IEmailSender emailService, ILogger<AutoCheckOut> logger, IConfiguration configuration, IOptions<SmtpSettings> smtpSettings)
        {
            _context = context;
            Configuration = configuration;
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
            _emailService = emailService;
        }
        public async Task AutoCheckOutAsync(string email, string subject, string htmlMessage)
        {
            string dateTimeForCheckinCheckoutSubject = "";

            subject = Configuration["EmailSenderSettings:Subject:AutoCheckOut"] + " on " + dateTimeForCheckinCheckoutSubject;
            try
            {
                var today = DateTime.Today;
                var yesterday = DateTime.Today.AddDays(-1);
                var employeeAttendance = (from emp in _context.EmployeeAttendance
                                          join employee in _context.Employees on emp.EmployeeId equals employee.EmployeeId
                                          where emp.CheckInTime != null &&
                                                emp.CheckInTime.Date == today
                                                || emp.CheckInTime.Date == yesterday
                                                && emp.CheckOutTime == null
                                          select new
                                          {
                                              EmployeeAttendance = emp,
                                              Employee = employee
                                          }).ToList();

                try
                {
                    string _employeehtmlMessage = "";
                    foreach (var emplyoee in employeeAttendance)
                    {
                        var empAttendance = (from em in _context.EmployeeAttendance
                                             where em.EmployeeId == emplyoee.Employee.EmployeeId &&
                                                   (em.CheckInTime != null && em.CheckInTime.Date == today && em.CheckOutTime == null
                                                   || em.CheckInTime.Date == yesterday
                                                   )
                                             select em).FirstOrDefault();

                        if (empAttendance != null)
                        {
                            empAttendance.CheckOutTime = emplyoee.EmployeeAttendance.CheckInTime.AddHours(9);
                            empAttendance.IsAutoCheckedOut = true;
                            empAttendance.CheckinMadeSystemIP = IPAddress.Loopback.ToString();

                            _context.EmployeeAttendance.Update(empAttendance);
                        }
                        var record = empAttendance;

                        var checkInTime = record.CheckInTime != null ? ((DateTime)record.CheckInTime).ToString("hh:mm tt", CultureInfo.InvariantCulture) : "Not Checked In";
                        var checkOutTime = record.CheckOutTime != null ? ((DateTime)record.CheckOutTime).ToString("hh:mm tt", CultureInfo.InvariantCulture) : "Not Checked Out";
                        string workingHours = CalculateWorkingHours(record.CheckInTime, record.CheckOutTime);


                        var employeeName = emplyoee.Employee.Name;
                        var systemIP = emplyoee.EmployeeAttendance.CheckinMadeSystemIP ?? "N/A";
                        var checkOutIP = emplyoee.EmployeeAttendance.CheckOutMadeSystemIP ?? "N/A";
                        var designation = emplyoee.Employee.Designation;
                        var checkOutType = emplyoee.EmployeeAttendance.IsAutoCheckedOut;


                        _employeehtmlMessage += $@"<div style='border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 15px; padding: 15px; background-color: #f9f9f9;'>
                                            <h3 style='margin: 0 0 10px;'>{employeeName}</h3>
                                            <h5 style='margin: 0 0 10px;'>{designation}</h5>
                                            <p style='margin: 5px 0;'><strong>Check-In Time:</strong> {checkInTime}</p>
                                            <p style='margin: 5px 0;'><strong>Check-Out Time:</strong> {checkOutTime}</p>
                                            <p style='margin: 5px 0;'><strong>Working Hours:</strong> {workingHours}</p>
                                            <p style='margin: 5px 0;'><strong>Check-In IP:</strong> {systemIP}</p>
                                          <p style='margin: 5px 0;'><strong>Check-Out Type IP:</strong> <button style='margin-top: 10px; padding: 10px 15px; background-color: #28a745; color: #ffffff; border: none; border-radius: 5px; cursor: pointer;'>
                                                Auto Check-Out
                                            </button></p>
                                           
                                        </div>";

                    }
                    await _context.SaveChangesAsync();
                    if (_employeehtmlMessage != "")
                    {

                        string siteUrl = Configuration["AppSettings:ThisSiteUrl"];
                        htmlMessage += $@"
                                         <p>Hi,</p>
                                         <p>Our system has detected that you did not check out properly. As a result, the system has automatically checked you out. Please note that this may affect your timesheet working hours.</p>
                                         <p>We kindly request you to ensure proper check-out in the future to avoid any discrepancies.</p>
                                            {_employeehtmlMessage}
                                         <p>Regards,</p>
                                         <p>HR Team<br/>Spark IT Tech</p>
                                         <p>Please visit <a href='{siteUrl}' style='color: #007bff; text-decoration: none;'>{Configuration["AppSettings:SiteTitle"]}</a> for more information.</p>";

                    }
                    if(htmlMessage != "")
                    {
                        _emailService.SendEmailAsync(email, subject, htmlMessage);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
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
