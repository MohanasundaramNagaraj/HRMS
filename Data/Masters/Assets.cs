using SparkHRMS.Data.Setting;
using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.Data.Masters
{
    public class AssetMaster
    {
        [Key]
        public int AssetId { get; set; }

        [Required]
        [MaxLength(100)]
        public string AssetName { get; set; }

        [MaxLength(50)]
        public string AssetCode { get; set; } // Unique Identifier (e.g., LAP-001)

        [Required]
        public DateTime PurchaseDate { get; set; }

        public decimal? PurchasePrice { get; set; }

        [MaxLength(100)]
        public string Vendor { get; set; }

        public int? WarrantyMonths { get; set; } // Warranty period

        public AssetStatus Status { get; set; } = AssetStatus.Available;

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        //public ICollection<AssetAllocation> Allocations { get; set; }
    }

    public enum AssetStatus
    {
        Available,
        Assigned,
        UnderMaintenance,
        Lost,
        Retired
    }

}

