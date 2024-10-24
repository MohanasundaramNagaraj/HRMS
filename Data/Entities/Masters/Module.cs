
using SparkHRMS.Data.Entities.EntityBase;
using SparkHRMS.Data.Entities.ProjectTransactions.StatusTransaction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkHRMS.Data.Entities.Masters
{
    [Table("Module")]
	public class Module : Entity
	{

		[Column(Order = 2)]
		[Required]
		[StringLength(30)]
		[MaxLength(30)]
        public string Code { get; set; }

		[Column(Order = 3)]
		[Required]
		[StringLength(50)]
		[MaxLength(50)]
        public string Name { get; set; }

		ICollection<ModuleStatus> ModuleStatuses { get; set; }

	}
}
