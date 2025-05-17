using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.Data.Entities
{
    public class Holiday
    {
        [Key]
        [Display(Name = "Holiday ID")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Holiday Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Holiday Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Recurring Every Year")]
        public bool IsRecurring { get; set; }

        [MaxLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Created By User ID")]
        public int? CreatedUserID { get; set; }
    }
}
