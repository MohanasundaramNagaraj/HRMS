using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SparkHRMS.Data.Entities.Masters;

namespace SparkHRMS.Seed
{
	public class ProjectUserRoleConfiguration : IEntityTypeConfiguration<ProjectUserRole>
	{
		public void Configure(EntityTypeBuilder<ProjectUserRole> builder)
		{
			builder.HasData(
				new ProjectUserRole
				{
					Id = 1,
					Name = "Business Analyst"
				},
				new ProjectUserRole
				{
					Id = 2,
					Name = "Technical Architect"
				},
				new ProjectUserRole
				{
					Id = 3,
					Name = "Project Manager"
				},
				new ProjectUserRole
				{
					Id = 4,
					Name = "Developer"
				},
				new ProjectUserRole
				{
					Id	= 5,
					Name = "Tester"
				}
			);
		}
	}
}
