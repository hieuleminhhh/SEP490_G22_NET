namespace EHM_API.DTOs.ReservationDTO.Guest
{
	public class ReservationByStatus
	{
		public int ReservationId { get; set; }

		public string? ConsigneeName { get; set; }
		public string? GuestPhone { get; set; }
		public DateTime? ReservationTime { get; set; }
		public int? GuestNumber { get; set; }
		public decimal Deposits { get; set; }
		public string? Note { get; set; }
		public int? StatusOfTable { get; set; }
		public int? Status { get; set; }
		public int? TableId { get; set; }
		public OrderDetailDTO3 Order { get; set; }



	}
	public class OrderDetailDTO3
	{
		public int OrderId { get; set; }
		public DateTime? OrderDate { get; set; }
		public int? Status { get; set; }
		public decimal? TotalAmount { get; set; }
		public string? Note { get; set; }
		public ICollection<OrderItemDTO3> OrderDetails { get; set; }
	}

	public class OrderItemDTO3
	{
		public int DishId { get; set; }
		public string? ItemName { get; set; }
		public int ComboId { get; set; }
		public string? NameCombo { get; set; }
		public decimal? Price { get; set; }
		public decimal? DiscountedPrice { get; set; }
		public decimal? UnitPrice { get; set; }
		public int? Quantity { get; set; }
		public string ImageUrl { get; set; }
	}

}
