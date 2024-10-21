

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data.Entities.Masters;
using SparkHRMS.Data.Entities.ProjectTransactions;
using SparkHRMS.Seed;


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
        public DbSet<EmailLogs> EmailLogs { get; set; }
        public DbSet<Holiday> Holiday { get; set; }


        #region Masters
        public DbSet<Module> Module { get; set; } = default!;
		#endregion

		#region ProjectTransactions
		public DbSet<ModuleStatus> ModuleStatuses { get; set; } = default!;
		public DbSet<Project> Projects { get; set; } = default!;
		public DbSet<Sprint> Sprints { get; set; } = default!;
		public DbSet<WorkItem> WorkItems { get; set; } = default!;
		public DbSet<Client> Clients { get; set; } = default!;
		#endregion



		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Client>().HasIndex(c => c.Name).IsUnique();

            builder.ApplyConfiguration(new ModuleConfiguration());

        }
    }
}
