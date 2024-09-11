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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationDbContext> _logger;
        private IConfiguration _configuration { get; }

        public DbInitializer(
            RoleManager<IdentityRole> roleManager,
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

                _roleManager.CreateAsync(new IdentityRole(Roles.RoleType.SuperAdmin.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.RoleType.Admin.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.RoleType.Employee.ToString())).GetAwaiter().GetResult();


                //Create superadmin user.
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = _configuration["UserSettings:SuperAdmin:UserName"],
                    Email = _configuration["UserSettings:SuperAdmin:UserName"],
                    PhoneNumber = _configuration["UserSettings:SuperAdmin:PhoneNumber"],
                }, _configuration["UserSettings:SuperAdmin:Password"]).GetAwaiter().GetResult();

                ApplicationUser superAdmin = _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == _configuration["UserSettings:SuperAdmin:UserName"]).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(superAdmin, Roles.RoleType.SuperAdmin.ToString()).GetAwaiter().GetResult();

            }
        }
    }
}
