using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SparkHRMS.Data.Entities;

namespace SparkHRMS.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // USERS
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users.Select(u => new { u.Id, u.UserName, u.Email }).ToList();
            return Json(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string userName, string email, string password)
        {
            var user = new ApplicationUser { UserName = userName, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            return Json(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(string id, string email)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = email;
            var result = await _userManager.UpdateAsync(user);
            return Json(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            return Json(result);
        }

        // ROLES
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList();
            return Json(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            ApplicationRole role = new ApplicationRole();
            role.Name = roleName;
            var result = await _roleManager.CreateAsync(role);
            return Json(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var result = await _roleManager.DeleteAsync(role);
            return Json(result);
        }

        // USER ROLE MAPPING
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            return Json(roles);
        }

        public IActionResult Index() => View();
    }

}
