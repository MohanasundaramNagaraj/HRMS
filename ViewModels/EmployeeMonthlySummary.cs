using SparkHRMS.Data.Entities;
using SparkHRMS.Utilities;

namespace SparkHRMS.ViewModels
{
    public class EmployeeMonthlySummaryWithStatusCount
    {
        public EmployeeDetailsDto EmployeeDetails { get; set; }
        public EmployeeAttendanceStatusCounts EmployeeAttendanceStatusCounts { get; set; }
    }
    public class EmployeeAttendanceStatusCounts
    {
        public int? EmployeePresentCountOnMonth { get; set; }
        public int? EmployeeHalfDayCountOnMonth { get; set; }
        public int? EmployeeLeaveCountOnMonth { get; set; }
        public int? EmployeeAbsentCountOnMonth { get; set; }
        public int? EmployeeWeekendCountOnMonth { get; set; }
        public int? EmployeeHolidayCountOnMonth { get; set; }
        public int? EmployeePermissionNeededCountOnMonth { get; set; }
        public int? EmployeePendingCheckOutNeededCountOnMonth { get; set; } 
    }

    public class EmployeeAttendanceResponseDto
    {
        public EmployeeDetailsDto EmployeeDetails { get; set; }
        public EmployeeMonthlySummary MonthlySummary { get; set; }
    }
    public class EmployeeDetailsDto
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string? Address { get; set; }
        public string? Designation { get; set; }
        public string? EmpCode { get; set; }
        public string? ReportingHeadMailID { get; set; }
    }
    public class EmployeeMonthlySummary
    {
        public List<EmployeeAttendanceDto> DailyAttendanceRecords { get; set; } = new List<EmployeeAttendanceDto>();
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
        public DateTime? Date { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public TimeOnly CheckInTime { get; set; }
        public TimeOnly CheckOutTime { get; set; }
        public string CheckInTimeInString { get; set; }
        public string CheckOutTimeInString { get; set; }
        public DateTime? CheckInDateTime { get; set; }
        public DateTime? CheckOutDateTime { get; set; }
        public string WorkingHours { get; set; } // e.g., "8 hours 15 mins"
        public AttendanceStatus Status { get; set; }
        public string IP { get; set; }
        public string CheckOutMadeSystemIP { get; set; }
        public string CheckInPosition { get; set; }
        public string CheckOutPosition { get; set; }
        public string CheckInLocation { get; set; }
        public string CheckOutLocation { get; set; }
    }

    public enum AttendanceStatus
    {
        Present,
        HalfDay,
        Absent,
        Weekend,
        Holiday,
        PermissionNeeded,
        PendingCheckOut
    }

    public class AttendanceSettings
    {
        public int FullDayThreshold { get; set; }
        public int HalfDayThreshold { get; set; }
        public int PermissionNeededThreshold { get; set; }
        public List<string> WeekendDays { get; set; }
    }
}
