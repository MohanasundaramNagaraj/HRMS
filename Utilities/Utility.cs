using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public static class Utility
    {

        private static readonly HttpClient client = new HttpClient();
        public static string CalculateWorkingHours(DateTime? checkInTime, DateTime? checkOutTime)
        {
            if (checkInTime.HasValue && checkOutTime.HasValue)
            {
                var duration = checkOutTime.Value - checkInTime.Value;
                return string.Format("{0:%h} hours {0:%m} mins", duration);
            }
            return string.Empty;
        }
        public static async Task<string> GetLocationFromCoordinates(string? Position)
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
    }
}
