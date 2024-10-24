using SparkHRMS.Dtos;

namespace SparkHRMS.ViewModels
{
	public class AllProjectVM : PaginationProperty
	{
        public List<ProjectVM> Projects { get; set; }
    }

	public class ProjectVM
	{
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public string Status { get; set; }
        public int ProgressPercentage { get; set; }
        public List<Member> Members { get; set; }
    }

    public class Member
    {
        public int Id { get; set; }
        public string Name  { get; set; }
    }
}
