using SparkHRMS.Data.Entities.EntityBase;
using SparkHRMS.Data.Entities.ProjectTransactions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.Masters
{
	[Table("ProjectUserRole")]
	public class ProjectUserRole : Entity
	{
		[Column(Order = 2)]
		[Length(2, 50)]
        public string Name { get; set; }

		public ICollection<EmployeeRole> EmployeeRoles { get; set; }

	 }
}
