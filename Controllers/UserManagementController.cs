using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;

namespace SparkHRMS.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return Json(users);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(UserViewModel model)
        {
            if (model.Id == 0)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }
            else
            {
                var user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user == null) return NotFound();

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> InactivateUser([FromBody] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            return Ok();
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

        public async Task<IActionResult> GetUserRolesList()
        {
            var userRoles = await (from u in _userManager.Users
                             join ur in _context.UserRoles on u.Id equals ur.UserId
                             join r in _roleManager.Roles on ur.RoleId equals r.Id
                             where ur != null
                             select new UserRoleViewModel
                             {
                                UserName = u.UserName,
                                RoleName = r.Name
                             }
                         ).ToListAsync();

            return Json(userRoles);
        }

        public IActionResult Index() => View();


    }

}

public class UserViewModel
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } // only for new user
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}

public class UserRoleViewModel
{
    public string UserName { get; set; }
    public string RoleName { get; set; }
}