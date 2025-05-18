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
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; } 

        public string? CheckinMadeSystemIP { get; set; }
        public string? CheckOutMadeSystemIP { get; set; }
        public bool IsCheckedOut => CheckOutTime.HasValue;

        public string? CheckInPosition { get; set; }
        public string? CheckOutPosition { get; set; }

        public bool? IsAutoCheckedOut { get; set; }

        //public bool IsPermission { get; set; }
        //public DateTime? PermissionStartTime { get; set; }
        //public DateTime? PermissionEndTime { get; set; }
        //public bool IsOnDuty { get; set; }
        //public DateTime? DutyStartTime { get; set; }
        //public DateTime? DutyEndTime { get; set; }
        //public bool IsLeave { get; set; }
        //public bool IsHalfDayLeave { get; set; }
        //public HalfDay? HalfDayLeave { get; set; }
        //public enum HalfDay
        //{
        //    FirstHalf,  
        //    SecondHalf  
        //}
    }
}
