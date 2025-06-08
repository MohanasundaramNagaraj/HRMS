using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities
{
    public class VisitorEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Pass Number")]
        [StringLength(50)]
        public string PassNo { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Entry Date")]
        [DataType(DataType.DateTime)]
        public DateTime EntryDate { get; set; }

        [Required]
        [Display(Name = "Gate In-Charge")]
        [StringLength(100)]
        public string GateInCharge { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        [Phone]
        public string ContactNumber { get; set; }

        [Display(Name = "Visitor Name")]
        [StringLength(100)]
        public string VisitorName { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Visitor Category")]
        [StringLength(50)]
        public string VisitorCategory { get; set; }

        [Display(Name = "Company Name")]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [Display(Name = "Purpose of Visit")]
        [StringLength(200)]
        public string PurposeOfVisit { get; set; }

        [Display(Name = "Person To Meet")]
        [StringLength(100)]
        public string PersonToMeet { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        [Display(Name = "ID Proof Type")]
        [StringLength(50)]
        public string IDProofType { get; set; }

        [Display(Name = "ID Proof Number")]
        [StringLength(100)]
        public string IDProofNumber { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        [Display(Name = "Visitor Image URL")]
        [DataType(DataType.ImageUrl)]
        public string VisitorImageUrl { get; set; }

        [Display(Name = "Vehicle Number")]
        [StringLength(20)]
        public string VehicleNumber { get; set; }

        [Display(Name = "Additional Visitor Count")]
        [Range(0, 100)]
        public int AdditionalVisitorCount { get; set; }

        [Display(Name = "Additional Visitor Names")]
        [StringLength(500)]
        public string AdditionalVisitorNames { get; set; }

        [Display(Name = "Recent Visits")]
        [Range(0, int.MaxValue)]
        public int RecentVisits { get; set; }

        [Required]
        [Display(Name = "In Date")]
        [DataType(DataType.DateTime)]
        public DateTime InTime { get; set; }

       
        [Display(Name = "Out Date")]
        [DataType(DataType.DateTime)]
        public DateTime? OutTime { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }


}
