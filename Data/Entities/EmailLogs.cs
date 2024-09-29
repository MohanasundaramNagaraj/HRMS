using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SparkHRMS.Data.Entities
{
    [Table("EmailLogs")]
    public class EmailLogs
    {
        [Key] 
        public int Id { get; set; }

        [Required]  
        [EmailAddress]
        public string Recipient { get; set; }

        [EmailAddress] 
        public string Cc { get; set; }

        [Required]  
        [MaxLength(255)]  
        public string Subject { get; set; }

        [Required]  
        public string Body { get; set; }

        [Required]  
        public DateTime SentDate { get; set; }

        [Required]  
        public bool IsSuccessful { get; set; }

        
        public string ErrorMessage { get; set; }

    }
}
