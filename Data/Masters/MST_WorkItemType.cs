using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Masters
{
    [Table("MST_WorkItemType")]
    public class MST_WorkItemType
    {
        [Key]
        public int ID { get; set; }
        public string WorkItemTypeName { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
