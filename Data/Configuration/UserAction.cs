using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SparkHRMS.Data.Configuration
{

    [Table("CFG_UsersAction")]
    public class UsersAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionID { get; set; }

        [Required]
        public string PageCode { get; set; }

        [Required]
        public string ActionCode { get; set; }

        public string PageName { get; set; }

        public string MenuCode { get; set; }
    }

}
