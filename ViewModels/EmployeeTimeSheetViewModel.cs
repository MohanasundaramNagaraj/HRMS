using SparkHRMS.Data.Entities;

namespace SparkHRMS.ViewModels
{
    public class EmployeeTimeSheetViewModel
    {
        public List<EmployeeAttendance> AttendanceDetails { get; set; }
        public EmployeeDetailsDto EmployeeDetails { get; set; }
        public List<Timesheet> TimeSheetRecords { get; set; }
    }
}
