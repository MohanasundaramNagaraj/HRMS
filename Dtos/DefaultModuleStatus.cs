namespace SparkHRMS.Dtos
{
	public class DefaultModuleStatus
	{
		public List<ModuleStatusDetail> Status { get; set; }
	}
	public class ModuleStatusDetail
	{
		public string Name { get; set; }
		public int DisplaySequence { get; set; }
	}
}
