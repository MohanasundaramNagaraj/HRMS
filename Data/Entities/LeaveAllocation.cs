using SparkHRMS.Data.Setting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities
{
    [Table("LeaveAllocation")]

    public class LeaveAllocation
    {
        public int Id { get; set; }
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int YearId { get; set; }

        [Required]
        public decimal TotalLeaveAllocated { get; set; }
        public decimal TotalLeaveBalance { get; set; }
        public decimal TotalUsedDays { get; set; }
        public decimal TotalCarriedForwardLeaves { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<LeaveAllocationDetail> LeaveDetails { get; set; } = new List<LeaveAllocationDetail>();
    }
}
