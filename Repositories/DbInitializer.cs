using Microsoft.AspNetCore.Identity;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using SparkHRMS.Interfaces;
using SparkHRMS.Utilities;
using Microsoft.EntityFrameworkCore;

namespace SparkHRMS.Repositories
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationDbContext> _logger;
        private IConfiguration _configuration { get; }

        public DbInitializer(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<ApplicationDbContext> logger
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async void Initialize()
        {
            //migrations if they are not applied
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                    _context.Database.Migrate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
            }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(Roles.RoleType.SuperAdmin.ToString()).GetAwaiter().GetResult())
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = Roles.RoleType.SuperAdmin.ToString();
                _roleManager.CreateAsync(role).GetAwaiter().GetResult();

                role = new ApplicationRole();
                role.Name = Roles.RoleType.Admin.ToString();
                _roleManager.CreateAsync(role).GetAwaiter().GetResult();

                role = new ApplicationRole();
                role.Name = Roles.RoleType.Employee.ToString();
                _roleManager.CreateAsync(role).GetAwaiter().GetResult();


                //Create superadmin user.
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = _configuration["UserSettings:SuperAdmin:UserName"],
                    Email = _configuration["UserSettings:SuperAdmin:UserName"],
                    EmailConfirmed = true,
                    PhoneNumber = _configuration["UserSettings:SuperAdmin:PhoneNumber"],
                    IsActive = true
                }, _configuration["UserSettings:SuperAdmin:Password"]).GetAwaiter().GetResult();

                ApplicationUser superAdmin = _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == _configuration["UserSettings:SuperAdmin:UserName"]).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(superAdmin, Roles.RoleType.SuperAdmin.ToString()).GetAwaiter().GetResult();

                //Create superadmin user.
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = _configuration["UserSettings:Admin:UserName"],
                    Email = _configuration["UserSettings:Admin:UserName"],
                    EmailConfirmed = true,
                    PhoneNumber = _configuration["UserSettings:Admin:PhoneNumber"],
                    IsActive = true
                }, _configuration["UserSettings:Admin:Password"]).GetAwaiter().GetResult();

                ApplicationUser Admin = _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == _configuration["UserSettings:Admin:UserName"]).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(superAdmin, Roles.RoleType.Admin.ToString()).GetAwaiter().GetResult();

            }
        }
    }
}
