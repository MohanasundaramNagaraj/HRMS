using SparkHRMS.Data.Entities.EntityBase;
using SparkHRMS.Data.Entities.Masters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.ProjectTransactions
{
    [Table("Project")]
    public class Project : Entity
    {

        [Column(Order = 2)]
        [Required]
        [StringLength(30)]
        [MaxLength(30)]
        public string Code { get; set; }

        [Column(Order = 3)]
        [Required]
        [StringLength(255)]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column(Order = 4)]
        [StringLength(4000)]
        [MaxLength(4000)]
        public string Description { get; set; }

        [Column(Order = 5)]
        [ForeignKey("Clients")]
        public int? ClientId { get; set; }   
        public Client Clients { get; set; }

        [Column(Order = 6)]
        [ForeignKey("Status")]
        public int? StatusId { get; set; }    // Entires on Project Status reference Id from Lookup Table filter Based on LookUp Type
        public ModuleStatus Status { get; set; }

        [Column(Order = 7)]
        [ForeignKey("CreatedUser")]
        public int CreatedBy { get; set; }
        public ApplicationUser CreatedUser { get; set; }


        [Column(Order = 8)]
        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        [Column(Order = 9)]
        [Required]
        public DateTime PlannedStartDate { get; set; }

        [Column(Order = 10)]
        [Required]
        public DateTime PlannedEndDate { get; set; }

        [Column(Order = 11)]
        public DateTime? ActualStartDate { get; set; } = null;

        [Column(Order = 12)]
        public DateTime? ActualEndDate { get; set; } = null;

        [Column(Order = 13)]
        public bool IsActive { get; set; } = true;

        [Column(Order = 14)]
        public bool IsDeleted { get; set; } = false;

        [Column(Order = 15)]
        [ForeignKey("DeletedUser")]
        public int? DeletedBy { get; set; } = null;
        public ApplicationUser DeletedUser { get; set; }

        [Column(Order = 16)]
        public DateTime? DeletedTime { get; set; } = null;


        ICollection<Sprint> Sprints { get; set; }
        ICollection<WorkItem> WorkItems {  get; set; }
	}
}
