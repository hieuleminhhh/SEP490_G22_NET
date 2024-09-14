using EHM_API.DTOs.DishDTO.Manager;

namespace EHM_API.DTOs.TableDTO.Manager
{
	public class FindTableAndGetOrderDTO
	{
		public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public int Status { get; set; }
		public DateTime? RecevingOrder { get; set; }
		public int? AccountId { get; set; }
		public int? InvoiceId { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? TotalDiscount { get; set; }
		public string? GuestPhone { get; set; }
		public decimal Deposits { get; set; }
		public int AddressId { get; set; }
		public string? GuestAddress { get; set; }
		public string? ConsigneeName { get; set; }
		public string? Note { get; set; }
		public int? Type { get; set; }
		public List<GetTableDTO> TableIds { get; set; }
		public List<TableOfOrderDetailDTO> OrderDetails { get; set; }
	}

	public class TableOfOrderDetailDTO
	{
		public int OrderDetailId { get; set; }
		public decimal? UnitPrice { get; set; }
        public int? DishesServed { get; set; }
        public int? Quantity { get; set; }
		public int? DishId { get; set; }
		public int? ComboId { get; set; }
		public SearchDishDTO? Dish { get; set; }
		public SearchComboDTO? Combo { get; set; }
	}

	public class GetTableDTO
	{
		public int TableId { get; set; }
		public int? Status { get; set; }
		public int? Capacity { get; set; }
		public string? Floor { get; set; }
	}
}
