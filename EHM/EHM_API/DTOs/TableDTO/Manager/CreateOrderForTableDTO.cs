namespace EHM_API.DTOs.TableDTO.Manager
{
	public class CreateOrderForTableDTO
	{
		public string? GuestAddress { get; set; }
		public string? ConsigneeName { get; set; }
		public DateTime OrderDate { get; set; }
		public int? Status { get; set; }
		public DateTime? RecevingOrder { get; set; }
		public string GuestPhone { get; set; }
		public int? AccountId { get; set; }
		public int? AddressId { get; set; }
		public string Note { get; set; }
		public int? Type { get; set; }
		public int? DiscountId { get; set; }
		public List<CreateOrderDetailDTO> OrderDetails { get; set; }
	}

	public class CreateOrderDetailDTO
	{
		public int? DishId { get; set; }
		public int? ComboId { get; set; }
		public decimal? UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal? DiscountedPrice { get; set; }

		public string? Note { get; set; }
		public DateTime? OrderTime { get; set; }
	}
}
