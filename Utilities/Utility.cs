using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SparkHRMS.Data;
using SparkHRMS.Data.Masters;
using SparkHRMS.Data.Setting;
using SQLitePCL;

namespace SparkHRMS.Utilities
{
    public class Location
    { 
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }
    public  class Utility
    {
        private  readonly ApplicationDbContext _context;
        public  Utility(ApplicationDbContext context)
        {
            _context = context;
        }

        private  readonly HttpClient client = new HttpClient();
        public  string CalculateWorkingHours(DateTime? checkInTime, DateTime? checkOutTime)
        {
            if (checkInTime.HasValue && checkOutTime.HasValue)
            {
                var duration = checkOutTime.Value - checkInTime.Value;
                return string.Format("{0:%h} hours {0:%m} mins", duration);
            }
            return string.Empty;
        }
        public  async Task<string> GetLocationFromCoordinates(string? Position)
        {
            if (!string.IsNullOrWhiteSpace(Position))
            {
                Location location = JsonConvert.DeserializeObject<Location>(Position);

                // Now you have lat and lng as double values
                double lat = location.Lat;
                double lng = location.Lng;
                string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={lat}&lon={lng}&zoom=18&addressdetails=1";

                client.DefaultRequestHeaders.Add("User-Agent", "C# App");

                // Send the request to the OSM Nominatim API
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(jsonString);

                    // Extracting specific details from the JSON response
                    var address = json["display_name"].ToString();
                    return address;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetEmployeeNameById(int EmployeeId)
        {
            string name =  _context.Employees.Where(x => x.EmployeeId == EmployeeId).Select(x => x.Name).FirstOrDefault();
            return name;
        } 
         
        public string GetEmployeeCodeById(int EmployeeId)
        {
            string name =  _context.Employees.Where(x => x.EmployeeId == EmployeeId).Select(x => x.EmployeeCode).FirstOrDefault();
            return name;
        }

        public AssetMaster GetAssetById(int AssetId)
        {
            return  _context.AssetMasters.Where(x => x.AssetId == AssetId).Select(x => x).FirstOrDefault();
        }   
        public SetYear GetYearById(int YearId)
        {
            return  _context.Year.Where(x => x.Id == YearId).Select(x => x).FirstOrDefault();
        }
    }
}
