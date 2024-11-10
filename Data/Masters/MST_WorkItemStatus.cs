using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Masters
{
    [Table("MST_WorkItemStatus")]
    public class MST_WorkItemStatus
    {
        [Key]
        public int ID { get; set; }
        public string WorkItemStatus { get; set; }
        public string Descreption { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set;}
        public DateTime? UpdatedDate { get; set;}

    }
}
