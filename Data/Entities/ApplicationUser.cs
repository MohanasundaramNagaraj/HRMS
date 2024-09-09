using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SparkHRMS.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {

        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }

        
        [ForeignKey("Role")]
        public string? RoleId { get; set; }
        public virtual IdentityRole Role { get; set; }
    }
}
