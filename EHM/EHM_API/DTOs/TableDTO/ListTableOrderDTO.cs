namespace EHM_API.DTOs.TableDTO
{
	public class ListTableOrderDTO
	{
		public int OrderId { get; set; }
		public DateTime? OrderDate { get; set; }
		public int? Status { get; set; }
		public DateTime? RecevingOrder { get; set; }
		public decimal? TotalAmount { get; set; }
		public string? GuestPhone { get; set; }
		public decimal Deposits { get; set; }
		public string? GuestAddress { get; set; }
		public string? ConsigneeName { get; set; }
		public List<TableOrderDTO> Tables { get; set; }


	}
	public class TableOrderDTO
	{
		public int TableId { get; set; }
		public int? Status { get; set; }
		public int? Capacity { get; set; }
		public string? Floor { get; set; }
        public string? Lable { get; set; }
    }
}
