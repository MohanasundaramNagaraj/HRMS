namespace SparkHRMS.Dtos
{
	public class CreateClientDto
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string CountryCode { get; set; }
		public string PhoneNumber { get; set; }  
		public string Description { get; set; }
		public string Address { get; set; }
	}
}
