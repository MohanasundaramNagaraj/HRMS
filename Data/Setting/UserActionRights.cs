using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Setting
{
   
    [Table("SET_UsersActionRights")]
    public class UsersActionRight
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RightsID { get; set; }

        [Required]
        [Column(TypeName = "varchar(max)")]
        public string PageCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(max)")]
        public string ActionCode { get; set; }

        public int? RoleID { get; set; }

        public int? UserID { get; set; }

        public bool? IsVisible { get; set; }

        public bool? IsEnabled { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }

}
