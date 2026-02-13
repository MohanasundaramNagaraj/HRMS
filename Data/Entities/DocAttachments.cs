using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SparkHRMS.Data.Entities
{
    [Table("Doc_Attachments")]
    public class DocAttachments
    {
        [Key]
        public int AttachmentID { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(255)]
        public string UniqueName { get; set; }

        [MaxLength(100)]
        public string? FileMIMEType { get; set; }

        public decimal? FileSizeInKb { get; set; }

        [MaxLength(500)]
        public string? Location { get; set; }

        public byte[]? Document { get; set; }

        [Required]
        public int DocumentID { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;

       
    }
}
