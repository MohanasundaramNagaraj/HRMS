namespace SparkHRMS.Dtos
{
	public class ProjectModuleStatusDto
	{
        public int StatusMappedId { get; set; }
        public int ItemId { get; set; }  // project,sprint,pbi,task,bug
        public int ModuleStatusId { get; set; }
        public int ModuleId { get; set; }
        public string Status { get; set; }
        public int DisplaySequence { get; set; }
        public bool IsActive { get; set; }
    }
}
