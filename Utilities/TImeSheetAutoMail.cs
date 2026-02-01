
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
using Org.BouncyCastle.Ocsp;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data.Setting;
using SparkHRMS.Interfaces;
using SparkHRMS.Services;
using SparkHRMS.ViewModels;
using System.Collections.Generic;
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
    public class TimeSheetAutoMail : IAutoTimeSheetMail
    {
        private readonly ApplicationDbContext _context;
        private readonly SmtpSettings _smtpSettings;
        private IConfiguration Configuration { get; }
        private readonly ILogger<AutoCheckOut> _logger;
        private readonly Interfaces.IEmailSender _emailService;
        public TimeSheetAutoMail(ApplicationDbContext context, Interfaces.IEmailSender emailService, ILogger<AutoCheckOut> logger, IConfiguration configuration, IOptions<SmtpSettings> smtpSettings)
        {
            _context = context;
            Configuration = configuration;
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
            _emailService = emailService;
        }
        public async Task AutoTimeSheetMailAsync(List<Employee> employees, string subject, string htmlMessage, List<EmailAddress> ccs)
        {

            try
            {
                List<EmployeeMailVM> emps = await getEmployeeBalanceDayCountAsync(employees);
                var today = DateTime.Today;

                string siteUrl = Configuration["AppSettings:ThisSiteUrl"];
                List<EmailAddress> tos = new List<EmailAddress>();
                string tableContent = "";
                foreach (var emp in emps)
                {
                    string tr = "<tr><td>" + emp.EmpName + "</td><td>" + emp.Destination + "</td><td>" + emp.BalanceDayscount + "</td></tr>";

                    tableContent = tableContent + tr;
                    var to = new EmailAddress(emp.EmailId, emp.EmpName);
                    tos.Add(to);
                }

                htmlMessage = @"
                    <p>Hi Team,</p>
                    <p>The following employees have not updated their timesheets. Please update your timesheet ASAP.</p>

                    <table border='1' cellpadding='8' cellspacing='0' style='border-collapse: collapse; font-family: Arial;'>
                        <thead style='background-color: #f2f2f2;'>
                            <tr>
                                <th>Employee Name</th>
                                <th> Destination</th>
                                <th>Timesheet Pending Days</th></tr></thead><tbody>" + tableContent + "</tbody></table><p>Regards,</p><p>HR Team<br/>Spark IT Tech</p>";


                string dateTimeForSubject = DateTime.Now.ToString("dd MMM yyyy");

                subject = Configuration["EmailSenderSettings:Subject:AutoTimeSheetMailOut"] + " on " + dateTimeForSubject;

                var apiKey = Configuration["EmailSenderSettings:SendGridAPIKey"];
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(Configuration["EmailSenderSettings:From"], Configuration["EmailSenderSettings:UserName"]);
                //var mailTo = (from emp in _context.Users);
                //var to = new EmailAddress(emp.EmpName);
                // var to = new EmailAddress(email, email);
                var plainTextContent = "";
                var htmlContent = htmlMessage;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(
                                        Configuration["EmailSenderSettings:From"],
                                        Configuration["EmailSenderSettings:UserName"],
                                        System.Text.Encoding.UTF8
                                    ),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };
                tos.ForEach(t => mailMessage.To.Add(t.Email));
                ccs.ForEach(cc => mailMessage.CC.Add(cc.Email));
                int logId = 0;
                if (emps.Count() > 0)
                {
                    // var singleRecipient = tos.FirstOrDefault();
                    var emailLogs = new EmailLogs
                    {
                        Recipient = string.Join(",", tos.Select(t => t.Email)), // multiple recipients
                                                                                //Recipient = singleRecipient.Email, // multiple recipients
                        Cc = string.Join(",", ccs.Select(c => c.Email)),
                        Subject = subject,
                        Body = htmlMessage,
                        SentDate = DateTime.Now,
                        IsSuccessful = false,
                        ErrorMessage = string.Empty
                    };

                    _context.EmailLogs.Add(emailLogs);
                    _context.SaveChanges();

                    logId = emailLogs.Id;
                }

                var smtpClient = new System.Net.Mail.SmtpClient(Configuration["EmailSenderSettings:SmtpServer"])
                {
                    Port = 587,
                    Credentials = new NetworkCredential(Configuration["EmailSenderSettings:From"], Configuration["EmailSenderSettings:Password"]),
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                try
                {

                    if (emps.Count() > 0)
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                        await Task.Delay(500);

                        var log = _context.EmailLogs.Where(x => x.Id == logId).FirstOrDefault();

                        log.IsSuccessful = true;

                        _context.Entry(log).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
        private async Task<List<EmployeeMailVM>> getEmployeeBalanceDayCountAsync(List<Employee> employees)
        {
            List<EmployeeMailVM> empDetls = new List<EmployeeMailVM>();

            var today = DateTime.Today;
            int yearId = today.Year;
            int monthId = today.Month;

            // Generate dates from the 1st to today
            var validDates = Enumerable.Range(1, today.Day)
                .Select(day => new DateTime(yearId, monthId, day))
                .ToList();

            // Fetch holidays once
            var holidays = await _context.Holiday
                .Where(h => h.Date.Month == monthId && h.Date.Year == yearId)
                .Select(h => h.Date.Date)
                .ToListAsync();

            // Remove holidays from valid working dates
            var workingDates = validDates
                .Where(date => !holidays.Contains(date))
                .ToList();



            foreach (var i in employees)
            {
                var timesheetData = await _context.Timesheets
                    .Where(t =>
                        t.Date.Month == monthId &&
                        t.Date.Year == yearId &&
                        t.EmployeeId == i.EmployeeId &&
                        !t.IsDeleted)
                    .GroupBy(t => t.Date.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalHours = g.Sum(x => x.HoursWorked)
                    })
                    .ToListAsync();
                var checkinDates = await _context.EmployeeAttendance
              .Where(a =>
                  a.EmployeeId == i.EmployeeId &&
                  a.CheckInTime.Month == monthId &&
                  a.CheckInTime.Year == yearId)
              .Select(a => a.CheckInTime.Date)
              .ToListAsync();


                var incompleteDays = workingDates
                    .Where(date =>
                    {
                        bool checkedIn = checkinDates.Contains(date);
                        var entry = timesheetData.FirstOrDefault(d => d.Date == date);
                        return checkedIn && (entry == null
                        //|| entry.TotalHours < 8
                        );
                    })
                    .ToList();

                int incompleteDayCount = incompleteDays.Count;

                if (incompleteDayCount != 0)
                {
                    empDetls.Add(new EmployeeMailVM
                    {
                        EmpName = i.Name,         // Assuming Employee model has Name
                        EmailId = i.Email,
                        BalanceDayscount = incompleteDayCount,
                        Destination = i.Designation
                    });
                }

            }

            return empDetls;
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
