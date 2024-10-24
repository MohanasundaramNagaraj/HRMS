using SparkHRMS.Dtos;

namespace SparkHRMS.ViewModels
{

    public class AllClientVM : PaginationProperty
	{
        public List<ClientVM> Clients { get; set; }
    }

    public class ClientVM
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
