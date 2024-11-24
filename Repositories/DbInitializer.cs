using Microsoft.AspNetCore.Identity;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using SparkHRMS.Interfaces;
using SparkHRMS.Utilities;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Masters;

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

        public async Task InitializeAsync()
        {
            // Migrations if they are not applied
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                    _context.Database.Migrate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
            }

            try
            {
                await SeedWorkItemStatusesAsync();
                await SeedWorkItemTypesAsync();
                await SeedSubStatusesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
            }

            // Create roles if they are not created
            if (!await _roleManager.RoleExistsAsync(Roles.RoleType.SuperAdmin.ToString()))
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = Roles.RoleType.SuperAdmin.ToString();
                await _roleManager.CreateAsync(role);

                role = new ApplicationRole();
                role.Name = Roles.RoleType.Admin.ToString();
                await _roleManager.CreateAsync(role);

                role = new ApplicationRole();
                role.Name = Roles.RoleType.Employee.ToString();
                await _roleManager.CreateAsync(role);

                // Create SuperAdmin user
                var superAdminUser = new ApplicationUser
                {
                    UserName = _configuration["UserSettings:SuperAdmin:UserName"],
                    Email = _configuration["UserSettings:SuperAdmin:UserName"],
                    EmailConfirmed = true,
                    PhoneNumber = _configuration["UserSettings:SuperAdmin:PhoneNumber"],
                    IsActive = true
                };

                var createSuperAdminResult = await _userManager.CreateAsync(superAdminUser, _configuration["UserSettings:SuperAdmin:Password"]);
                if (createSuperAdminResult.Succeeded)
                {
                    var superAdmin = await _userManager.FindByEmailAsync(_configuration["UserSettings:SuperAdmin:UserName"]);
                    await _userManager.AddToRoleAsync(superAdmin, Roles.RoleType.SuperAdmin.ToString());
                }

                // Create Admin user
                var adminUser = new ApplicationUser
                {
                    UserName = _configuration["UserSettings:Admin:UserName"],
                    Email = _configuration["UserSettings:Admin:UserName"],
                    EmailConfirmed = true,
                    PhoneNumber = _configuration["UserSettings:Admin:PhoneNumber"],
                    IsActive = true
                };

                var createAdminResult = await _userManager.CreateAsync(adminUser, _configuration["UserSettings:Admin:Password"]);
                if (createAdminResult.Succeeded)
                {
                    var admin = await _userManager.FindByEmailAsync(_configuration["UserSettings:Admin:UserName"]);
                    await _userManager.AddToRoleAsync(admin, Roles.RoleType.Admin.ToString());
                }
            }

        }

        private async Task SeedWorkItemStatusesAsync()
        {
            var superAdmin = _userManager.FindByEmailAsync(_configuration["UserSettings:SuperAdmin:UserName"]).GetAwaiter().GetResult();

            if (superAdmin == null)
            {
                _logger.LogError("SuperAdmin user not found. Cannot seed WorkItemStatus without a valid CreatedBy user.");
                return;
            }

            if (!await _context.Set<MST_WorkItemStatus>().AnyAsync())
            {
                var workItemStatuses = new[]
                {
                    new MST_WorkItemStatus { WorkItemStatus = "Completed", Descreption = "Work item has been completed", IsActive = true, CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new MST_WorkItemStatus { WorkItemStatus = "Taken For Development", Descreption = "Work item is taken for development", IsActive = true, CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new MST_WorkItemStatus { WorkItemStatus = "In Progress", Descreption = "Work item is currently in progress", IsActive = true, CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new MST_WorkItemStatus { WorkItemStatus = "Not yet Started", Descreption = "Work item has not been started yet", IsActive = true, CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now }
                };

                await _context.Set<MST_WorkItemStatus>().AddRangeAsync(workItemStatuses);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedWorkItemTypesAsync()
        {
            var superAdmin = await _userManager.FindByEmailAsync(_configuration["UserSettings:SuperAdmin:UserName"]);

            if (superAdmin == null)
            {
                _logger.LogError("SuperAdmin user not found. Cannot seed WorkItemType without a valid CreatedBy user.");
                return;
            }

            if (!await _context.Set<MST_WorkItemType>().AnyAsync())
            {
                var workItemTypes = new[]
                {
                    new MST_WorkItemType { WorkItemTypeName = "Bug", Description = "An issue or defect in the product", IsActive = true, CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new MST_WorkItemType { WorkItemTypeName = "Product Backlog Item", Description = "An item in the product backlog", IsActive = true, CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new MST_WorkItemType { WorkItemTypeName = "Task", Description = "A task to be completed", IsActive = true, CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new MST_WorkItemType { WorkItemTypeName = "Deployment", Description = "A deployment action", IsActive = true, CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new MST_WorkItemType { WorkItemTypeName = "Unit Test", Description = "A unit test case", IsActive = true, CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now }
                };

                await _context.Set<MST_WorkItemType>().AddRangeAsync(workItemTypes);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedSubStatusesAsync()
        {
            var superAdmin = await _userManager.FindByEmailAsync(_configuration["UserSettings:SuperAdmin:UserName"]);

            if (superAdmin == null)
            {
                _logger.LogError("SuperAdmin user not found. Cannot seed SubStatusMaster without a valid CreatedBy user.");
                return;
            }

            if (!await _context.Set<SubStatusMaster>().AnyAsync())
            {
                var subStatuses = new[]
                {
                    new SubStatusMaster { SubStatus = "Pending", IsActive = true, StatusColor = "#FFA500", ActionName = "Awaiting Approval", CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new SubStatusMaster { SubStatus = "Approve", IsActive = true, StatusColor = "#008000", ActionName = "Approved", CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new SubStatusMaster { SubStatus = "Reject", IsActive = true, StatusColor = "#FF0000", ActionName = "Rejected", CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now },
                    new SubStatusMaster { SubStatus = "Hold", IsActive = true, StatusColor = "#FFFF00", ActionName = "On Hold", CreatedBy = superAdmin.Id, CreatedDate = DateTime.Now }
                };

                await _context.Set<SubStatusMaster>().AddRangeAsync(subStatuses);
                await _context.SaveChangesAsync();
            }
        }
    }
}
