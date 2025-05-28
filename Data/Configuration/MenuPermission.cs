using SparkHRMS.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.Data.Configuration
{
    [Table("CFG_MenuPermission")]
    public class MenuPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionID { get; set; }

        [Required]
        [StringLength(200)]
        public string MenuCode { get; set; }

        public int? RoleID { get; set; }

        public int? UserID { get; set; }

        [StringLength(20)]
        public string Permission { get; set; }
    }
}
