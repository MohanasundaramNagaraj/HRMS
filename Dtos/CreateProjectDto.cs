namespace SparkHRMS.Dtos
{
	public class CreateProjectDto
	{
		public string Name { get; set; }

		public string ProjectClientName { get; set; }

		public string Description { get; set; }

		public DateTime PlannedStartDate { get; set; }

		public DateTime PlannedEndDate { get; set; }
	}
}
