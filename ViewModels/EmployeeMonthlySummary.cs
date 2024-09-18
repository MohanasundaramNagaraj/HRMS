using SparkHRMS.Data.Entities;

namespace SparkHRMS.ViewModels
{
    public class EmployeeAttendanceResponseDto
    {
        public EmployeeDetailsDto EmployeeDetails { get; set; }
        public EmployeeMonthlySummary MonthlySummary { get; set; }
    }
    public class EmployeeDetailsDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string? Address { get; set; }
        public string? Designation { get; set; }
        public string? EmpCode { get; set; }
    }
    public class EmployeeMonthlySummary
    {
        public List<DailyAttendanceDto> DailyAttendanceRecords { get; set; } = new List<DailyAttendanceDto>();
        public string TotalWorkingHoursThisMonth { get; set; }  
    }
    public class DailyAttendanceDto
    {
        public DateTime Date { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string WorkingHours { get; set; } 
    }

    public class EmployeeAttendanceDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string WorkingHours { get; set; } // e.g., "8 hours 15 mins"
        public string IP { get; set; }
    }
}
