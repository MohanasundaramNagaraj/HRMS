using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SparkHRMS.Data.Entities;
using System.Security.Claims;
namespace SparkHRMS.Services
{

    public class UserResolverService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserResolverService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        internal string GetUser()
        {
            if (_httpContextAccessor != null
                && _httpContextAccessor.HttpContext != null
                && _httpContextAccessor.HttpContext.User != null
                && _httpContextAccessor.HttpContext.User.Identity != null)
            {
                return _httpContextAccessor.HttpContext.User?.Identity?.Name;
            }
            else
            {
                return string.Empty;
            }

        }

        internal int? GetUserId() // int will  be null if the user is not authenticated
        {
            var userIdClaimValue = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdClaimValue))
            {
                return int.Parse(userIdClaimValue);
            }
            else
            {
                return null;
            }

        }

        internal string? GetUserName()
        {
            var userNameClaimValue = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(userNameClaimValue))
            {
                return userNameClaimValue;
            }
            else
            {
                return null;
            }

        }

        internal string? GetRole()
        {
            var userNameClaimValue = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userNameClaimValue))
            {
                return userNameClaimValue;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<ApplicationUser>> GetAdminsAndSuperAdminsAsync()
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            // Combine the lists (remove duplicates if necessary)
            var result = superAdmins.Union(admins).ToList();

            return result;
        }
    }
}
