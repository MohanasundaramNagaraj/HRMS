using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities
{
    [Table("LeaveRequestHistory")]
    public class LeaveRequestHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("LeaveRequest")]
        [Display(Name = "Leave Request")]
        public int LeaveRequestId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [Required]
        [Display(Name = "Change Date")]
        public DateTime ChangeDate { get; set; }

        [Required]
        [Display(Name = "Changed By")]
        public int ChangedBy { get; set; }

        [ForeignKey("LeaveRequestId")]
        public virtual LeaveRequest LeaveRequest { get; set; }
    }
}
