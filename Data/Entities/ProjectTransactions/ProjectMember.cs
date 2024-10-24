using SparkHRMS.Data.Entities.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.ProjectTransactions
{
	[Table("ProjectMember")]
	public class ProjectMember : Entity
	{
		[Column(Order = 2)]
		[ForeignKey("Project")]
		public int ProjectId { get; set; }
		public Project Project { get; set; }

		[Column(Order = 3)]
		[ForeignKey("Member")]
		public int EmployeeId { get; set; } 
		public Employee Member { get; set; }

		public ICollection<EmployeeRole> EmployeeRoles { get; set; }
	}
}
