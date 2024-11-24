using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities
{
    [Table("EmployeeAttendanceRequest")]
    public class EmployeeAttendanceRequest
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        public string RequestType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public string Reason { get; set; } 
        public DateTime CreatedTime { get; set; }
        public int? StatusUpdatedBy { get; set; }
        public DateTime? StatusUpdatedTime { get; set; }
    }
}
