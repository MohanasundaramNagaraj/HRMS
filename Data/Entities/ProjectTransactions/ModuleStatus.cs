using SparkHRMS.Data.Entities.EntityBase;
using SparkHRMS.Data.Entities.Masters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.ProjectTransactions
{
	[Table("ModuleStatus")]
	public class ModuleStatus : Entity
	{

        [Column(Order = 2)]
        [ForeignKey("Modules")]
        public int? ModuleId { get; set; }
        public Module Modules { get; set; }

        [Column(Order = 3)]
        [Required]
        [StringLength(255)]
        [MaxLength(255)]
        public string Status { get; set; }

        [Column(Order = 4)]
        [Required]
        public int DisplaySequence { get; set; }

        [Column(Order = 5)]
        [Required]
        public bool IsActive { get; set; } = true;


        ICollection<Project> Projects { get; set; }
        ICollection<Sprint> Sprints { get; set; }
        ICollection<WorkItem> WorkItems { get; set; }
    }
}
