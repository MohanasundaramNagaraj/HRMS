using SparkHRMS.Data.Entities.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.ProjectTransactions
{
	[Table("EmployeeReporting")]
	public class EmployeeReporting : Entity
	{
		[Column(Order = 2)]
		public int ReportingById { get; set; }
		[ForeignKey("ReportingById")]
		public Employee ReportingByEmployee { get; set; }  // The employee who is reporting


		[Column(Order = 3)]
		public int ReportingToId { get; set; }
		[ForeignKey("ReportingToId")]
		public Employee ReportingToEmployee { get; set; }  // The employee being reported to
	}
}
