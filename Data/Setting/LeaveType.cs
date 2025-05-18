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

        [Required]
        [MaxLength(1000)]
        [Display(Name = "Description")]
        public string Description { get; set; } // Optional Description

        [Required]
        [Display(Name = "Maximum Allowed Days Per Year")]
        public int MaxAllowedDaysPerYear { get; set; }

        [Display(Name = "Applicable After Working Days")]
        public int ApplicableAfterWorkingDays { get; set; }

        [Display(Name = "Max Consecutive Leave Days Per Month")]
        public int MaxConsecutiveLeaveAllowedDaysPerMonth { get; set; }

        [Display(Name = "Carry Forward Allowed")]
        public bool IsCarryForward { get; set; } = false;

        [Display(Name = "Leave Without Pay")]
        public bool IsLeaveWithoutPay { get; set; } = false;

        [Display(Name = "Partially Paid Leave")]
        public bool IsPartiallyPaidLeave { get; set; } = false;

        [Display(Name = "Optional Leave")]
        public bool IsOptionalLeave { get; set; } = false;

        [Display(Name = "Allow Negative Balance")]
        public bool AllowNegativeBalance { get; set; } = false;

        [Display(Name = "Include Holidays With Leaves")]
        public bool IncludeHolidaysWithLeaves { get; set; }

        [Display(Name = "Compensatory Leave")]
        public bool IsCompensatory { get; set; } = false;

        [Display(Name = "Encashment Leave")]
        public bool IsEncashmentLeave { get; set; } = false;

        [Display(Name = "Earned Leave")]
        public bool IsEarnedLeave { get; set; } = false;

        public bool IsActive { get; set; } = true; // Active status
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
