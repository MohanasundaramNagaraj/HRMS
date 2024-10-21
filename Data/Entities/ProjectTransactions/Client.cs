
using SparkHRMS.Data.Entities.EntityBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.ProjectTransactions
{
	[Table("Client")]
	public class Client : Entity
    {
		[Column(Order = 2)]
		[Required]
		[StringLength(50)]
		[MaxLength(50)]
        public string Name { get; set; }

		[Column(Order = 3)]
		[StringLength(4000)]
		[MaxLength(4000)]
        public string? Description { get; set; } = null;

		[Column(Order = 4)]
		[StringLength(255)]
		[MaxLength(255)]
		public string? Email { get; set; } = null;

        [Column(Order = 5)]
        [StringLength(20)]
        [MaxLength(20)]
        public string? CountryCode { get; set; } = null;

        [Column(Order = 6)]
		[StringLength(20)]
		[MaxLength(20)]
		public string? PhoneNumber { get; set; } = null;

		[Column(Order = 7)]
		[StringLength(4000)]
		[MaxLength(4000)]
		public string? Address { get; set; } = null;

		[Column(Order = 8)]
		public bool IsActive { get; set; } = true;

		[Column(Order = 9)]
		[ForeignKey("CreatedUser")]
		public int CreatedBy { get; set; } 
		public ApplicationUser CreatedUser { get; set; }

		[Column(Order = 10)]
		[Required]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		[Column(Order = 11)]
		[ForeignKey("ModifiedUser")]
		public int? ModifiedBy { get; set; } = null;
        public ApplicationUser ModifiedUser { get; set; }

		[Column(Order = 12)]
		public DateTime? ModifiedTime { get; set; } = DateTime.Now;


        [Column(Order = 13)]
		[Required]
		public bool IsDeleted { get; set; } = false;

        [Column(Order = 14)]
        public DateTime DeletedDateTime { get; set; }

		[Column(Order = 15)]
		[ForeignKey("DeletedUser")]
		public int? DeletedBy { get; set; } = null;
        public ApplicationUser DeletedUser { get; set; }



        ICollection<Project> Projects { get; set; }
    }
}
