using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.Data.Entities
{
    [Table("EmployeeAttendance")]
    public class EmployeeAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public ApplicationUser Employee { get; set; }

        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public string? CheckinMadeSystemIP { get; set; }
        public bool IsCheckedOut => CheckOutTime.HasValue;
    }
}
