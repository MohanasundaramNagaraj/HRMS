using SparkHRMS.Data.Entities.EntityBase;
using SparkHRMS.Data.Entities.Masters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Odbc;

namespace SparkHRMS.Data.Entities.ProjectTransactions
{
	[Table("Sprint")]
	public class Sprint : Entity
	{

		[Column(Order = 2)]
		[ForeignKey("Projects")]
		public int? ProjectId { get; set; }
		public Project Projects { get; set; }

		[Column(Order = 3)]
		[ForeignKey("Status")]
		public int? StatusId { get; set; }    // Entires on Sprint Status reference Id from Lookup Table filter Based on LookUp Type
		public ModuleStatus Status { get; set; }

		[Column(Order = 4)]
		[Required]
		[StringLength(50)]
		[MaxLength(50)]
		public string Name { get; set; }

		[Column(Order = 5)]
		[StringLength(4000)]
		[MaxLength(4000)]
		public string? Description { get; set; } = null;

		[Column(Order = 6)]
		[Required]
        public DateTime PlannedStartDateTime { get; set; }

		[Column(Order = 7)]
		[Required]
        public DateTime PlannedEndDateTime { get; set; }

		[Column(Order = 8)]
		public DateTime? ActualStartDateTime { get; set; } = null;

		[Column(Order = 9)]
		public DateTime? ActualEndDateTime { get; set; } = null;

		[Column(Order = 10)]
		[ForeignKey("CreatedUser")]
		public int CreatedBy { get; set; }
		public ApplicationUser CreatedUser { get; set; }


		[Column(Order = 11)]
		[Required]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		[Column(Order = 12)]
		public bool IsDeleted { get; set; } = false;

		[Column(Order = 13)]
		[ForeignKey("DeletedUser")]
		public int? DeletedBy { get; set; } = null;
		public ApplicationUser DeletedUser { get; set; }

		[Column(Order = 14)]
		public DateTime? DeletedTime { get; set; } = null;


		ICollection<WorkItem> WorkItems { get; set; }
	}
}
