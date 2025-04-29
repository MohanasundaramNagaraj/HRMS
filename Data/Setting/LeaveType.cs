using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Setting
{
    [Table("SET_LeaveType")]
    public class LeaveType
    {
        [Key]
        public int LeaveTypeId { get; set; } // Primary Key

        [Required]
        [MaxLength(100)]
        public string Code { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; } // Leave Type Name (e.g., Sick Leave, Casual Leave)

        [MaxLength(1000)]
        public string Description { get; set; } // Optional Description

        [Required]
        public int MaxAllowedDaysPerYear { get; set; } // Maximum leave days allowed per year
        public int ApplicableAfterWorkingDays { get; set; } // Days after which leave is applicable
        public int MaxConsecutiveLeaveAllowedDaysPerMonth { get; set; } // Maximum leave days allowed per month

        public bool IsCarryForward { get; set; } = false;
        public bool IsLeaveWithoutPay { get; set; } = false;
        public bool IsPartiallyPaidLeave { get; set; } = false;
        public bool IsOptionalLeave { get; set; } = false;
        public bool AllowNegativeBalance { get; set; } = false;
        public bool IncludeHolidaysWithLeaves { get; set; }
        public bool IsCompensatory { get; set; } = false;
        public bool IsEncashmentLeave { get; set; } = false;
        public bool IsEarnedLeave { get; set; } = false;

        public bool IsActive { get; set; } = true; // Active status
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
