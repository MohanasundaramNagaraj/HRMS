using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SparkHRMS.Data.Logs
{
    [Table("Logs")]
    public class Serilog_Logs
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(4000)]
        public string Message { get; set; }

        [MaxLength(1000)]
        public string MessageTemplate { get; set; }

        [MaxLength(128)]
        public string Level { get; set; }

        [Required]
        public DateTimeOffset TimeStamp { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Exception { get; set; } = "";

        [Column(TypeName = "nvarchar(max)")]
        public string? Properties { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? LogEvent { get; set; }
    }
}
