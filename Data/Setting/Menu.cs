using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SparkHRMS.Data.Setting
{
  
    [Table("SET_Menu")]
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuID { get; set; }

        [Required]
        [MaxLength(200)]
        public string MenuCode { get; set; }

        public string MenuName { get; set; }

        public int? ParentMenuId { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool IsSubMenu { get; set; } = false;
    }

}
