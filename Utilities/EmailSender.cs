
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

namespace SparkHRMS.Utilities
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        private IConfiguration Configuration { get; }
        private readonly ILogger<EmailSender> _logger;
        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration, IOptions<SmtpSettings> smtpSettings)
        {
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

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(Configuration["EmailSenderSettings:From"]));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            var smtpClient = new System.Net.Mail.SmtpClient(Configuration["EmailSenderSettings:SmtpServer"])
            {
                Port = 587,
                Credentials = new NetworkCredential(Configuration["EmailSenderSettings:From"], Configuration["EmailSenderSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(Configuration["EmailSenderSettings:From"]), // Update the sender email here
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                await Task.Delay(500); // Delay in milliseconds
            }
            catch (Exception ex)
            {
                // log an error message or throw an exception or both.
                _logger.LogError(ex.Message);
                throw;
            }
        }

    }

}
