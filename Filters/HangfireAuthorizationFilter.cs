using Hangfire.Dashboard;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SparkHRMS.Utilities;
using SparkHRMS.Services;

namespace SparkHRMS.Filters
{

    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IServiceProvider _services;
        public HangfireAuthorizationFilter(IServiceProvider services)
        {
            _services = services;

        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            return IsInRoleAdmin(context.GetHttpContext().User.Identity is { IsAuthenticated: true }).Result;

        }

        private async Task<bool> IsInRoleAdmin(bool isAuthenticatedUser)
        {
            if (isAuthenticatedUser)
            {
                using (var dbContextScope = _services.CreateScope())
                {
                    var userContext = dbContextScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var userId = userContext.Users.GetService<UserResolverService>().GetUserId();

                    using (var userManagerScope = _services.CreateScope())
                    {
                        var userManager = userManagerScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                        var user = await userManager.FindByIdAsync(userId.ToString());
                       
                        if (await userManager.IsInRoleAsync(user, Roles.RoleType.SuperAdmin.ToString()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }
    }
}

