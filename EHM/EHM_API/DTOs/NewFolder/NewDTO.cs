namespace EHM_API.DTOs.NewFolder
{
	public class NewsDTO
	{
		public int NewsId { get; set; }
		public string? NewsTitle { get; set; }
		public string? NewsContent { get; set; }
		public DateTime? NewsDate { get; set; }
		public int? AccountId { get; set; }
	}
}
