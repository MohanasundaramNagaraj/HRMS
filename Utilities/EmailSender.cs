
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
using IEmailSender = SparkHRMS.Interfaces.IEmailSender;
using SparkHRMS.Data.Entities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SparkHRMS.Data;
using Microsoft.EntityFrameworkCore;
using Hangfire.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;
using Serilog;

namespace SparkHRMS.Utilities
{
    public class EmailSender : IEmailSender
    {
        private readonly ApplicationDbContext _context;
        private readonly SmtpSettings _smtpSettings;
        private IConfiguration Configuration { get; }
        private readonly ILogger<EmailSender> _logger;
        public EmailSender(ApplicationDbContext context, ILogger<EmailSender> logger, IConfiguration configuration, IOptions<SmtpSettings> smtpSettings)
        {
            _context = context;
            Configuration = configuration;
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string dateTimeForCheckinCheckoutSubject = "";
            if (subject == "CheckIn" || subject == "CheckOut")
            {
                dateTimeForCheckinCheckoutSubject = DateTime.Now.ToString("dd MMM yyyy");
            }
            subject = Configuration["EmailSenderSettings:Subject:" + subject] + dateTimeForCheckinCheckoutSubject;

            //var emailMessage = new MimeMessage();
            //emailMessage.From.Add(MailboxAddress.Parse(Configuration["EmailSenderSettings:From"]));
            //emailMessage.To.Add(MailboxAddress.Parse(email));
            //emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            //var smtpClient = new System.Net.Mail.SmtpClient(Configuration["EmailSenderSettings:SmtpServer"])
            //{
            //    Port = 587,
            //    Credentials = new NetworkCredential(Configuration["EmailSenderSettings:From"], Configuration["EmailSenderSettings:Password"]),
            //    EnableSsl = true,
            //    UseDefaultCredentials = false,
            //    DeliveryMethod = SmtpDeliveryMethod.Network
            //};

            //var mailMessage = new MailMessage
            //{
            //    From = new MailAddress(Configuration["EmailSenderSettings:From"]), // Update the sender email here
            //    Subject = subject,
            //    Body = htmlMessage,
            //    IsBodyHtml = true,
            //};

            //mailMessage.To.Add(email);
           
            var apiKey = Configuration["EmailSenderSettings:SendGridAPIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(Configuration["EmailSenderSettings:From"], Configuration["EmailSenderSettings:UserName"]);
           
            var to = new EmailAddress(email, email);
            var plainTextContent = "";
            var htmlContent = htmlMessage;

            var emailLogs = new EmailLogs
            {
                Recipient = email,
                Cc = string.Empty,                          //Code Later if want
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
                // await smtpClient.SendMailAsync(mailMessage);
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

    }

}
