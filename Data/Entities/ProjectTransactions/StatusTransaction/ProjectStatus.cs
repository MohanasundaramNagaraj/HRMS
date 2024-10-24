using SparkHRMS.Data.Entities.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.ProjectTransactions.StatusTransaction
{
    [Table("ProjectStatus")]
    public class ProjectStatus : Entity
    {
        [Column(Order = 2)]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Column(Order = 3)]
        public int? ModuleStatusId { get; set; }
        public ModuleStatus ModuleStatus { get; set; }
    }
}
