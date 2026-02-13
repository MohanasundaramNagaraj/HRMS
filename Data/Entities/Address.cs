using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SparkHRMS.Data.Entities
{
    [Table("MST_Address")]
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address1 { get; set; }

        [MaxLength(500)]
        public string? Address2 { get; set; }

        [Required]
        [MaxLength(300)]
        public string City { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        [MaxLength(300)]
        public string Country { get; set; }

        [Required]
        [MaxLength(10)]
        public string Pincode { get; set; }

        [Required]
        public bool IsPermanentAddress { get; set; }

      
        [Required]
        [MaxLength(500)]
        public string DocumentType { get; set; }

       
        [Required]
        public int DocumentId { get; set; }

     
    }
}

