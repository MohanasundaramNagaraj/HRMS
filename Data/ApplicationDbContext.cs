

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data.Entities.Masters;
using SparkHRMS.Data.Entities.ProjectTransactions;
using SparkHRMS.Data.Entities.ProjectTransactions.StatusTransaction;
using SparkHRMS.Dtos;
using SparkHRMS.Seed;
using System.Reflection.Emit;


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
        public DbSet<ProjectUserRole> ProjectUserRoles { get; set; } = default!;
		#endregion

		#region ProjectTransactions
		public DbSet<ModuleStatus> ModuleStatuses { get; set; } = default!;
		public DbSet<Project> Projects { get; set; } = default!;
		public DbSet<Sprint> Sprints { get; set; } = default!;
		public DbSet<WorkItem> WorkItems { get; set; } = default!;
		public DbSet<Client> Clients { get; set; } = default!;
		public DbSet<ProjectStatus> ProjectStatuses { get; set; } = default!;
		public DbSet<SprintStatus> SprintStatuses { get; set; } = default!;
		public DbSet<ProjectMember> ProjectMembers { get; set; } = default!;
		public DbSet<EmployeeReporting> EmployeeReportings { get; set; } = default!;
		public DbSet<EmployeeRole> EmployeeRoles { get; set; } = default!;
		#endregion



		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Client>().HasIndex(c => c.Name).IsUnique();

			builder.Entity<Employee>()
				.HasMany(e => e.ReportingByEmployees)
				.WithOne(er => er.ReportingByEmployee)
				.HasForeignKey(er => er.ReportingById)
				.OnDelete(DeleteBehavior.Restrict); // Or Cascade, depending on your needs

			builder.Entity<Employee>()
				.HasMany(e => e.ReportingToEmployees)
				.WithOne(er => er.ReportingToEmployee)
				.HasForeignKey(er => er.ReportingToId)
				.OnDelete(DeleteBehavior.Restrict); // Or Cascade, depending on your needs

			builder.Entity<ProjectMember>()
				.HasOne(pm => pm.Project)
				.WithMany(p => p.ProjectMembers)
				.HasForeignKey(pm => pm.ProjectId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<ProjectMember>()
				.HasOne(pm => pm.Member)
				.WithMany(e => e.ProjectMembers)
				.HasForeignKey(pm => pm.EmployeeId)
				.OnDelete(DeleteBehavior.Restrict);

			


			builder.ApplyConfiguration(new ModuleConfiguration());
			builder.ApplyConfiguration(new ProjectUserRoleConfiguration());

		}


		#region #DB Context functions


		internal Module? GetModule(string Code)
		{
			var query = from m in this.Module
						where m.Code == Code
						select m;

			return query.FirstOrDefault() ?? null;
		}

        internal async Task<IEnumerable<ProjectModuleStatusDto>> GetProjectStatus(int projectId)
        {
            var query = from ms in this.ModuleStatuses
                        join ps in this.ProjectStatuses on ms.Id equals ps.ModuleStatusId
                        where ps.ProjectId == projectId 
                        select new ProjectModuleStatusDto
						{
							StatusMappedId = ps.Id,
                            ItemId = ps.ProjectId,
                            ModuleStatusId = ps.ModuleStatusId.Value,
                            ModuleId = ms.ModuleId.Value,
                            Status = ms.Status,
                            DisplaySequence = ms.DisplaySequence,
                            IsActive = ms.IsActive
                        };

            return await query.ToListAsync();
        }


        internal async Task UpdateStatus(string status, string moduleCode, int itemId)
        {

			switch (moduleCode)
			{
				case Constants.ModuleConstants.ModuleCode.Project:
					await UpdateProjectStatus(status, itemId);
					break;
				case Constants.ModuleConstants.ModuleCode.Sprint:
					await UpdateSprintStatus(status, itemId);
					break;
				case Constants.ModuleConstants.ModuleCode.ProductBackLog:
					await UpdateBackLogStatus(status, itemId);
					break;
				case Constants.ModuleConstants.ModuleCode.Task:
					await UpdateTaskStatus(status, itemId);
					break;
				case Constants.ModuleConstants.ModuleCode.Bug:
					await UpdateBugStatus(status, itemId);
					break;
				default:
					throw new InvalidOperationException("Invalid module code");
			}

			await SaveChangesAsync();
		}

		#endregion


		#region Private Helper Functions

		private async Task UpdateProjectStatus(string status, int projectId)
		{
			var projectStatuses = await GetProjectStatus(projectId);
			var project = await Projects.FirstOrDefaultAsync(p => p.Id == projectId);

			if (project != null)
			{
				project.StatusId = projectStatuses.FirstOrDefault(s => s.Status == status)?.ModuleStatusId;
				Entry(project).State = EntityState.Modified;
			}
		}

		private async Task UpdateSprintStatus(string status, int sprintId)
		{
			// Logic for updating Sprint Status
		}

		private async Task UpdateBackLogStatus(string status, int backlogId)
		{
			// Logic for updating Backlog Status
		}

		private async Task UpdateTaskStatus(string status, int taskId)
		{
			// Logic for updating Task Status
		}

		private async Task UpdateBugStatus(string status, int bugId)
		{
			// Logic for updating Bug Status
		}

		#endregion
	}
}
