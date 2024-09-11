using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SparkHRMS.Data.Entities
{
    [Table("MST_Employee")]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public string? EmployeeCode { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public DateTime DOB { get; set; } 
        public string Gender { get; set; }

        public string Designation { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string? Address { get; set; }

        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
