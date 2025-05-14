using SparkHRMS.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("LeaveRequestDetail")]
public class LeaveRequestDetail
{
    [Key]
    public int Id { get; set; }

    
    [ForeignKey("LeaveRequest")]
    public int LeaveRequestId { get; set; }

    [Required]
    public int LeaveTypeId { get; set; }

    [Required]
    public decimal AllocatedDays { get; set; }

    [Required]
    public decimal UsedDays { get; set; }

    [Required]
    public decimal BalanceDays { get; set; }

    [Required]
    public decimal RequiredDays { get; set; }

    public bool IsActive { get; set; } = true;

}
