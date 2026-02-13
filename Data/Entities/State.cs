using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities
{
    [Table("MST_State")]
    public class State
    {
        [Key]
        [Column("StateId")]
        public int StateId { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("StateCode")]
        public string StateCode { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("StateName")]
        public string StateName { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;
    }
}
