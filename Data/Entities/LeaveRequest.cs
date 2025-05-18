using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities
{
    [Table("LeaveRequest")]
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Is Start Date Half Day")]
        public bool IsStartDateHalfDay { get; set; } = false;

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Is End Date Half Day")]
        public bool IsEndDateHalfDay { get; set; } = false;

        [Required]
        [Display(Name = "Total Leave Days")]
        public decimal TotalLeaveDays { get; set; }

        [Required]
        [Display(Name = "Leave Reason")]
        public int LeaveReasonId { get; set; }

        [StringLength(500)]
        [Required]
        public string Comments { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; }


        public ICollection<LeaveRequestDetail> LeaveRequestDetails { get; set; }
    }
}
