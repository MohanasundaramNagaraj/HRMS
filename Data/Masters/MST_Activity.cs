using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Masters
{
    [Table("MST_Activity")]
    public class MST_Activity
    {
        [Key]
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
       
        

    }
}

