using SparkHRMS.Data.Entities.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.ProjectTransactions.StatusTransaction
{
    [Table("SprintStatus")]
    public class SprintStatus : Entity
    {
        [Column(Order = 2)]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Column(Order = 3)]
        [ForeignKey("Sprint")]
        public int? SprintId { get; set; }
        public Sprint Sprint { get; set; }

        [Column(Order = 4)]
        [ForeignKey("ModuleStatus")]
        public int? ModuleStatusId { get; set; }
        public ModuleStatus ModuleStatus { get; set; }
    }
}
