using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.Data.Entities
{
    public class Holiday
    {
        [Key]  
        public int Id { get; set; }

        [Required] 
        [MaxLength(100)]  
        public string Name { get; set; }

        [Required]  
        public DateTime Date { get; set; }

        [Required]  
        public bool IsRecurring { get; set; }

        [MaxLength(500)]  
        public string Description { get; set; }

        [Required]  
        public DateTime CreatedAt { get; set; }

        public int? CreatedUserID { get; set; }
    }
}
