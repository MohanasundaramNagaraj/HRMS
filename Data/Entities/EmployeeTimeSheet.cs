using SparkHRMS.Data.Masters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities
{
    [Table("EmployeeTimeSheet")]
    public class EmployeeTimeSheet
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public int WorkItemTypeID { get; set; }
        public int? DevOPsID { get; set; }
        //[ForeignKey("ID")]
        //public MST_WorkItemType WorkItemType { get; set; }
        public string Activity { get; set; }
        public string Descreption { get; set; }
        public int WorkItemStatusID { get; set; }
        //[ForeignKey("ID")]
        //public MST_WorkItemStatus WorkItemStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
