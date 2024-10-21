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
						Code = "P",
						Name = "Project"
					},
					new Module
					{
						Id = 2,
						Code = "S",
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
						Code = "T",
						Name = "Task"
					},
					new Module
					{
						Id = 5,
						Code = "B",
						Name = "BUG"
					}
				);
		}
	}
}
