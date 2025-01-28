using SparkHRMS.Data.Entities;

namespace SparkHRMS.ViewModels
{
    public class EmployeeTimeSheetViewModel
    {
        public EmployeeDetailsDto EmployeeDetails { get; set; }
        public List<Timesheet> TimeSheetRecords { get; set; }
    }
}
