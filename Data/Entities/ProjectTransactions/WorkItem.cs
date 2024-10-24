using SparkHRMS.Data.Entities.EntityBase;
using SparkHRMS.Data.Entities.Masters;
using SparkHRMS.Data.Entities.ProjectTransactions.StatusTransaction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.ProjectTransactions
{
    [Table("WorkItem")]
	public class WorkItem : Entity
	{

		[Column(Order = 2)]
		public int? DevOopsWorkItemId { get; set; } = null;

		[Column(Order = 3)]
		[ForeignKey("Projects")]
        public int? ProjectId { get; set; }
        public Project Projects { get; set; }

		[Column(Order = 4)]
		[ForeignKey("Sprints")]
		public int? SprintId { get; set; }
		public Sprint Sprints { get; set; }

        [Column(Order = 5)]
		[Required]
		[StringLength(255)]
		[MaxLength(255)]
        public string Title { get; set; }

		[Column(Order = 6)]
		[StringLength(4000)]
		[MaxLength(4000)]
		public string? Description { get; set; } = null;


		[Column(Order = 7)]
		[ForeignKey("Status")]
		public int? StatusId { get; set; }    
		public ModuleStatus Status { get; set; }


		[Column(Order = 8)]
		[ForeignKey("CreatedUser")]
		public int CreatedBy { get; set; }
		public ApplicationUser CreatedUser { get; set; }


		[Column(Order = 9)]
		[Required]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		[Column(Order = 10)]
		public bool IsDeleted { get; set; } = false;

		[Column(Order = 11)]
		[ForeignKey("DeletedUser")]
		public int? DeletedBy { get; set; } = null;
		public ApplicationUser DeletedUser { get; set; }

		[Column(Order = 12)]
		public DateTime? DeletedTime { get; set; } = null;
	}
}
