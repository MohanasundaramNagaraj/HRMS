
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data.Logs;
using SparkHRMS.Data.Masters;

namespace SparkHRMS.Data
{
    public class ApplicationDbContext :  IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAttendance> EmployeeAttendance { get; set; }
        public DbSet<EmployeeAttendanceRequest> EmployeeAttendanceRequest { get; set; }
        public DbSet<EmailLogs> EmailLogs { get; set; }
        public DbSet<Holiday> Holiday { get; set; }
        public DbSet<Serilog_Logs> Logs { get; set; }

        public DbSet<MST_WorkItemType> WorkItemType { get; set; }
        public DbSet<MST_WorkItemStatus> WorkItemStatus { get; set; }
        public DbSet<SubStatusMaster> SubStatus { get; set; }
        public DbSet<EmployeeTimeSheet> EmployeeTimeSheet { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
    }
}
