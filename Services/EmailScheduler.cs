using Hangfire;
using SparkHRMS.Services;

public class EmailScheduler
{
    private readonly IConfiguration _configuration;

    public EmailScheduler(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void ScheduleEmailJob()
    {
        var checkInCron = _configuration["EmailSenderSettings:JobSchedules:CheckInJobCron"];
        var checkOutCron = _configuration["EmailSenderSettings:JobSchedules:CheckOutJobCron"];
        var autoCheckOutCron = _configuration["EmailSenderSettings:JobSchedules:AutoCheckOutJobCron"];
        var autoTimeSheetCron = _configuration["EmailSenderSettings:JobSchedules:AutoTimeSheetMailJobCron"];

        //RecurringJob.AddOrUpdate<EmailQueueManager>(
        //    "CheckInJob",
        //    job => job.SendScheduledEmail("CheckIn"),
        //    checkInCron);


        //RecurringJob.AddOrUpdate<EmailQueueManager>(
        //   "AutoCheckOutJob",
        //   job => job.AutoCheckOutEmail("CheckOut"),
        //   checkInCron);

        //RecurringJob.AddOrUpdate<EmailQueueManager>(
        // "AutoMailTimeSheetJob",
        // job => job.AutoTimeSheetEmail("TimeSheet"),
        // autoTimeSheetCron);


    }
}