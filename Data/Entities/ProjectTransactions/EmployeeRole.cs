using SparkHRMS.Data.Entities.EntityBase;
using SparkHRMS.Data.Entities.Masters;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.ProjectTransactions
{
	[Table("EmployeeRole")]
	public class EmployeeRole : Entity
	{
        [Column(Order = 2)]
		[ForeignKey("Project")]
        public int? ProjectId { get; set; }
		public Project Project { get; set; }

        [Column(Order = 3)]
        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Column(Order = 4)]
        [ForeignKey("Member")]
        public int? MemberId { get; set; }
        public ProjectMember Member { get; set; }

        [Column(Order = 5)]
        [ForeignKey("ProjectUserRole")]
        public int? ProjectRoleId { get; set; }
        public ProjectUserRole ProjectUserRole { get; set; }

    }
}
