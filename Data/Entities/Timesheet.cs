using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.Data.Entities
{
    [Table("Timesheet")] 
    public class Timesheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UniqueId { get; set; }

        [Required]
        public int YearId { get; set; }

        [Required]
        public int MonthId { get; set; } 
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(500)]
        public string Day { get; set; } = string.Empty;

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(500)]
        public string TaskType { get; set; } = string.Empty;

        [StringLength(500)]
        public string Task { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Activity { get; set; } = string.Empty;

        [Required]
        [StringLength(int.MaxValue)]
        public string Descreption { get; set; } = string.Empty;

        [Required]
        [Range(0, 24)]
        public decimal HoursWorked { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
