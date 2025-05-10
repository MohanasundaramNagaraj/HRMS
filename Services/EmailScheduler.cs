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
        // Schedule job for 11:00 AM with a unique identifier
        RecurringJob.AddOrUpdate<EmailQueueManager>(
            "CheckInJob", // Unique identifier for the "CheckIn" job
            job => job.SendScheduledEmail("CheckIn"),
            checkInCron); // At 11:00 AM every day


        RecurringJob.AddOrUpdate<EmailQueueManager>(
           "AutoCheckOutJob", // Unique identifier for the "CheckIn" job
           job => job.AutoCheckOutEmail("CheckOut"),
           checkInCron);

        RecurringJob.AddOrUpdate<EmailQueueManager>(
         "AutoMailTimeSheetJob", // Unique identifier for the "CheckIn" job
         job => job.AutoTimeSheetEmail("TimeSheet"),
         autoTimeSheetCron);

        // Schedule job for 11:00 PM with a unique identifier
        //RecurringJob.AddOrUpdate<EmailQueueManager>(
        //    "CheckOutJob", // Unique identifier for the "CheckOut" job
        //    job => job.SendScheduledEmail("CheckOut"),
        //    checkOutCron); // At 11:00 PM every da
    }
}