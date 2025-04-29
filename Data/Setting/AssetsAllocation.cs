using SparkHRMS.Data.Masters;
using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.Data.Setting
{
    public class AssetAllocation
    {
        [Key]
        public int AllocationId { get; set; }

        [Required]
        public int AssetId { get; set; }
        //public AssetMaster Asset { get; set; }

        [Required]
        public int EmployeeId { get; set; } // FK from Employee Table

        [Required]
        public DateTime AllocationDate { get; set; } = DateTime.UtcNow;

        public DateTime? ReturnDate { get; set; }

        public bool IsReturned { get; set; } = false;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }

}
