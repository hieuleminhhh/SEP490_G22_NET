namespace EHM_API.DTOs.TableDTO
{
    public class TableAllDTO
    {
		public int TableId { get; set; }
		public int? Status { get; set; }
		public int? Capacity { get; set; }
		public string? Floor { get; set; }
        public string? Lable { get; set; }
    }
}
