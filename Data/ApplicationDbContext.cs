
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Configuration;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data.Logs;
using SparkHRMS.Data.Masters;
using SparkHRMS.Data.Setting;

namespace SparkHRMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
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
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<SetYear> Year { get; set; }
        public DbSet<Month> Month { get; set; }
        public DbSet<AssetMaster> AssetMasters { get; set; }
        public DbSet<AssetAllocation> AssetAllocations { get; set; }
        public DbSet<AssetMaintenance> AssetMaintenances { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveReasons> LeaveReasons { get; set; }
        public DbSet<LeaveAllocation> LeaveAllocations { get; set; }
        public DbSet<LeaveAllocationDetail> LeaveAllocationDetails { get; set; }
        public DbSet<SET_NumberConfig> SET_NumberConfig { get; set; }    
        public DbSet<MST_Activity> MST_Activities { get; set; }
       
        public DbSet<LeaveRequest> LeaveRequest { get; set; }
        public DbSet<LeaveRequestDetail> LeaveRequestDetail { get; set; }
        public DbSet<LeaveRequestHistory> LeaveRequestHistory { get; set; }
        public DbSet<VisitorEntry> VisitorEntries { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<UsersActionRight> UsersActionRight { get; set; }
        public DbSet<MenuPermission> MenuPermission { get; set; }
        public DbSet<UsersAction> UsersAction { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Month>().HasData(
                new Month { Id = 1, Name = "January", Number = 1 },
                new Month { Id = 2, Name = "February", Number = 2 },
                new Month { Id = 3, Name = "March", Number = 3 },
                new Month { Id = 4, Name = "April", Number = 4 },
                new Month { Id = 5, Name = "May", Number = 5 },
                new Month { Id = 6, Name = "June", Number = 6 },
                new Month { Id = 7, Name = "July", Number = 7 },
                new Month { Id = 8, Name = "August", Number = 8 },
                new Month { Id = 9, Name = "September", Number = 9 },
                new Month { Id = 10, Name = "October", Number = 10 },
                new Month { Id = 11, Name = "November", Number = 11 },
                new Month { Id = 12, Name = "December", Number = 12 }
            );

            modelBuilder.Entity<LeaveAllocation>()
           .HasMany(x => x.LeaveDetails)
           .WithOne()
           .HasForeignKey(d => d.LeaveAllocationId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Menu>()
                .HasIndex(e => e.MenuCode)
                .IsUnique();
        }
    }
}
