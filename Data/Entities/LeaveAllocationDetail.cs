using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities
{
    [Table("LeaveAllocationDetail")]
    public class LeaveAllocationDetail
    {
        public int Id { get; set; }
        public int LeaveAllocationId { get; set; }
        [Required]
        public int LeaveTypeId { get; set; }
        [Required]
        public DateTime FromDate { get; set; }
        [Required]
        public DateTime ToDate { get; set; }
        public string Description  { get; set; }
        [Required]
        public int AllocatedDays { get; set; }

        [Required]
        public int UsedDays { get; set; }
        public int RemainingDays { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

}
