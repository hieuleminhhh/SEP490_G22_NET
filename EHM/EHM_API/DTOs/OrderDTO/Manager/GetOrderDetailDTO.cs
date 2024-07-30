namespace EHM_API.DTOs.OrderDTO.Manager
{
	public class GetOrderDetailDTO
	{
		public int? ComboId { get; set; }
        public decimal? DiscountedPrice { get; set; }
		public int? DishId { get; set; }
		public int? DishesServed { get; set; }
        public string? ImageUrl { get; set; }
        public string? ItemName { get; set; }
		public string? NameCombo { get; set; }

		public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }	
		public string? Note { get; set; }
		public DateTime? OrderTime { get; set; }
	}
}
