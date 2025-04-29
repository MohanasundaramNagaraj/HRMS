using SparkHRMS.Data.Masters;
using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.Data.Setting
{
    public class AssetMaintenance
    {
        [Key]
        public int MaintenanceId { get; set; }

        [Required]
        public int AssetId { get; set; }

        public DateTime MaintenanceDate { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string IssueDescription { get; set; }

        [MaxLength(100)]
        public string MaintenanceProvider { get; set; }

        public decimal? MaintenanceCost { get; set; }

        public bool IsResolved { get; set; } = false;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }

}
