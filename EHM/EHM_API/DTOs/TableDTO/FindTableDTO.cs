namespace EHM_API.DTOs.TableDTO
{
	public class FindTableDTO
	{
		public int TableId { get; set; }
		public int? Status { get; set; }
		public int? Capacity { get; set; }
		public string? Floor { get; set; }
        public string? Lable { get; set; }

        // Danh sách các bàn được ghép
        public List<FindTableDTO> CombinedTables { get; set; }
	}
}
