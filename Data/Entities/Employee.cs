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

        public string FatherName { get; set; }
        public string Designation { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Image")] 
        public string? ImageUrl { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime? DateOfReleving { get; set; }
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }


        public string? ReportingHeadMailID { get; set; }
        [MaxLength(5)]
        public string? BloodGroup { get; set; }   


        // Personal Details
        [MaxLength(50)]
        public string? Nationality { get; set; }

        [MaxLength(20)]
        public string? MaritalStatus { get; set; }   

        [Phone]
        [MaxLength(15)]
        public string? AlternateMobileNumber { get; set; }

        [MaxLength(100)]
        public string? MotherName { get; set; }

        [MaxLength(100)]
        public string? SpouseName { get; set; }

        public int? NumberOfDependents { get; set; }

       
        [MaxLength(100)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(50)]
        public string? EmergencyContactRelation { get; set; }

        [Phone]
        [MaxLength(15)]
        public string? EmergencyContactNumber { get; set; }

        [Phone]
        [MaxLength(15)]
        public string? EmergencyAlternateNumber { get; set; }

       
        [MaxLength(12)]
        public string? AadhaarNumber { get; set; }

        [MaxLength(10)]
        public string? PANNumber { get; set; }

        [MaxLength(20)]
        public string? PassportNumber { get; set; }

        public DateTime? PassportExpiryDate { get; set; }

        [MaxLength(20)]
        public string? DrivingLicenseNumber { get; set; }


    }
}
