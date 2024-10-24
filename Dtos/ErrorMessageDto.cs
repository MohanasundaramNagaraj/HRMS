namespace SparkHRMS.Dtos
{
	public class ErrorMessageDto
	{
        public int StatusCode { get; set; }
        public Dictionary<string, dynamic> Error { get; set; }
    }
}
