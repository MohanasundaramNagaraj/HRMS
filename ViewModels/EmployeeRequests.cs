namespace SparkHRMS.ViewModels
{
    public class EmployeeRequests
    {
        public int EmployeeID { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string RequestType { get; set; }
        public string RequestDateTime { get; set; }
        public string Reason { get; set; }
        public string ApprovalStatus { get; set; }
    }
}
