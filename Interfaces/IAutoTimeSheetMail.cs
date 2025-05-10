using SendGrid.Helpers.Mail;
using SparkHRMS.Data.Entities;

namespace SparkHRMS.Interfaces
{
    public interface IAutoTimeSheetMail
    {
        Task AutoTimeSheetMailAsync(List<Employee> employees, string subject, string htmlMessage,List<EmailAddress> ccs);
    }
}
