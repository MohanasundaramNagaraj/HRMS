using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
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
    public class Utility
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public Utility(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly HttpClient client = new HttpClient();
        public string CalculateWorkingHours(DateTime? checkInTime, DateTime? checkOutTime)
        {
            if (checkInTime.HasValue && checkOutTime.HasValue)
            {
                var duration = checkOutTime.Value - checkInTime.Value;
                return string.Format("{0:%h} hours {0:%m} mins", duration);
            }
            return string.Empty;
        }
        public async Task<string> GetLocationFromCoordinates(string? Position)
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

        public string GetUserNameById(int? UserId)
        {
            if(UserId == null)
            {
                return string.Empty;
            }
            string name = _userManager.Users.Where(x => x.Id == UserId).Select(x => x.UserName).FirstOrDefault();
            return name;
        }

        public string GetEmployeeNameById(int EmployeeId)
        {
            string name = _context.Employees.Where(x => x.EmployeeId == EmployeeId).Select(x => x.Name).FirstOrDefault();
            return name;
        }

        public Employee GetEmployeeById(int EmployeeId)
        {
            return _context.Employees.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
        }

        public string GetEmployeeCodeById(int EmployeeId)
        {
            string name = _context.Employees.Where(x => x.EmployeeId == EmployeeId).Select(x => x.EmployeeCode).FirstOrDefault();
            return name;
        }

        public AssetMaster GetAssetById(int AssetId)
        {
            return _context.AssetMasters.Where(x => x.AssetId == AssetId).Select(x => x).FirstOrDefault();
        }
        public SetYear GetYearById(int YearId)
        {
            return _context.Year.Where(x => x.Id == YearId).Select(x => x).FirstOrDefault();
        }

        public LeaveType GetLeaveTypeById(int LeaveTypeId)
        {
            return _context.LeaveTypes.Where(x => x.LeaveTypeId == LeaveTypeId).Select(x => x).FirstOrDefault();
        }
        public LeaveReasons GetLeaveReasonById(int LeaveReasonId)
        {
            return _context.LeaveReasons.Where(x => x.LeaveReasonId == LeaveReasonId).Select(x => x).FirstOrDefault();
        }

        public string GenerateDocumentNumber(string documentType, bool commit, out string documentNumber)
        {
            int? nConfigId = null;
            int? startNumber = null;
            int? endNumber = null;
            int? paddingNumber = null;
            int? lastNumberGenerated = null;
            string prefix = null;
            string suffix = null;
            int nextNumber = 0;

            var configQuery = from y in _context.Year
                              join nc in _context.SET_NumberConfig on y.Id equals nc.YearID
                              where nc.DocumentType == documentType &&
                                    DateTime.Now >= y.StartDate &&
                                    DateTime.Now <= y.EndDate
                              select new
                              {
                                  nc.NConfigID,
                                  nc.StartNumber,
                                  nc.EndNumber,
                                  nc.PaddingNumber,
                                  nc.LastNumberGenerated,
                                  nc.Prefix,
                                  nc.Suffix
                              };

            var config = configQuery.FirstOrDefault();

            if (config != null)
            {
                nConfigId = config.NConfigID;
                startNumber = config.StartNumber;
                endNumber = config.EndNumber;
                paddingNumber = config.PaddingNumber;
                lastNumberGenerated = config.LastNumberGenerated;
                prefix = config.Prefix;
                suffix = config.Suffix;
            }

            if (nConfigId == null)
            {
                throw new Exception("Number configuration does not exist for " + documentType);
            }

            if (lastNumberGenerated >= endNumber)
            {
                throw new Exception("Last number reached for " + documentType);
            }

            if (lastNumberGenerated == null)
            {
                nextNumber = startNumber ?? 0;
            }
            else
            {
                nextNumber = lastNumberGenerated.Value + 1;
            }

            if (nextNumber > endNumber)
            {
                throw new Exception("Next number should not be greater than end number for " + documentType);
            }

            documentNumber = prefix +
                                    new string('0', paddingNumber ?? 0 - nextNumber.ToString().Length) +
                                    nextNumber +
                                    suffix;

            if (commit)
            {
                var configToUpdate = _context.SET_NumberConfig.Find(nConfigId);
                if (configToUpdate != null)
                {
                    configToUpdate.LastNumberGenerated = nextNumber;
                    _context.SaveChanges();
                }
            }

            return documentNumber;
        }

        public static async Task<bool> GetMenusPermissionAsync(string menuCode, int? userId,IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(menuCode) || userId == null) return false;

            //using var scope = serviceProvider.CreateScope();
            //var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //int roleId = await _context.UserRoles
            //                .Where(x => x.UserId == userId)
            //                .Select(x => x.RoleId)
            //                .FirstOrDefaultAsync();

            //var result = await (from m in _context.Menu
            //                    join mp in _context.MenuPermission on m.MenuCode equals mp.MenuCode
            //                    where (mp.UserID == userId || (mp.RoleID != null && mp.RoleID == roleId))
            //                    && m.MenuCode == menuCode
            //                    select mp.Permission).FirstOrDefaultAsync();

            //  return result != null ? result.ToUpper() == "GRANT" : false;

            return true;
        }

    }
}
