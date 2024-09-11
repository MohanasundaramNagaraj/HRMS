using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SparkHRMS.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; }
    }
}
