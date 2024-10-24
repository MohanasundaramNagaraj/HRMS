using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SparkHRMS.Data.Entities.Masters;


namespace SparkHRMS.Seed
{
	public class ModuleConfiguration : IEntityTypeConfiguration<Module>
	{
		public void Configure(EntityTypeBuilder<Module> builder)
		{
			builder.HasData
				(
					new Module
					{
						Id = 1,
						Code = "PRO",
						Name = "Project"
					},
					new Module
					{
						Id = 2,
						Code = "SPR",
						Name = "Sprint"
					},
					new Module
					{
						Id = 3,
						Code = "PBI",
						Name = "Product Backlog Item"
					},
					new Module
					{
						Id = 4,
						Code = "TSK",
						Name = "Task"
					},
					new Module
					{
						Id = 5,
						Code = "BUG",
						Name = "Bug"
					}
				);
		}
	}
}
