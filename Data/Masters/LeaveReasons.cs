using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Masters
{
    [Table("MST_LeaveReasons", Schema = "Masters")]
    public class LeaveReasons
    {
        [Key]
        public int LeaveReasonId { get; set; }
        public string LeaveReasonName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

    }
}
