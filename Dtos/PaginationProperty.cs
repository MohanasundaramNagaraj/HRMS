namespace SparkHRMS.Dtos
{
	public class PaginationProperty
	{
		public int TotalNumberOfPage { get; set; }
		public int CurrentPageNumber { get; set; }
		public int PreviousPageNumber { get; set; }
		public int NextPageNumber { get; set; }
        public int DataCount { get; set; }
    }
}
