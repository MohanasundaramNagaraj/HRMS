using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SparkHRMS.Data.Setting
{
    [Table("SET_NumberConfig")]
    public class SET_NumberConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int NConfigID { get; set; }
        [Required]
        [StringLength(200)]
        public string DocumentType { get; set; }
        [Required]
        public int StartNumber { get; set; }
        [Required]
        public int EndNumber { get; set; }
        [Required]
        public int PaddingNumber { get; set; }
        [Required]
        public int LastNumberGenerated { get; set; }
        [Required]
        [StringLength(200)]
        public string Prefix { get; set; }
        [Required]
        [StringLength(200)]
        public string Suffix { get; set; }
        [Required]
        public int YearID { get; set; }
        public bool IsActive { get; set; } = false;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        public string? ModifiedReason { get; set; }
    }
}
