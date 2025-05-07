namespace SparkHRMS.ViewModels
{
    public class ActivityViewModel
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedUserName { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
    
}
