using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Masters
{
    [Table("SubStatusMaster")]
    public class SubStatusMaster
    {
        [Key]
        public int ID { get; set; }
        public string SubStatus { get; set; }
        public bool IsActive { get; set; }
        public string StatusColor { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
