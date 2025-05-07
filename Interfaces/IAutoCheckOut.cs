namespace SparkHRMS.Interfaces
{
    public interface IAutoCheckOut
    {
        Task AutoCheckOutAsync(string email, string subject, string htmlMessage);
    }
}
