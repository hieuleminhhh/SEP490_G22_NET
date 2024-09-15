using EHM_API.DTOs.OrderDTO.Guest;

namespace EHM_API.DTOs.ReservationDTO.Guest
{
	public class ReservationDetailDTO
	{
		public int ReservationId { get; set; }
		public DateTime? ReservationTime { get; set; }
		public int? GuestNumber { get; set; }
		public string? Note { get; set; }
		public int? Status { get; set; }
	
		public string? GuestPhone { get; set; }
		public string? ConsigneeName { get; set; }
		public string? GuestAddress { get; set; }
		public string? Email { get; set; }
		public OrderDetailDTO1 Order { get; set; }


		public ICollection<TabledetailDTO> TableOfReservation { get; set; }
	}

	public class TabledetailDTO
	{
		public int? TableId { get; set; }
		public int? Capacity { get; set; }
		public string? Floor { get; set; }
        public string? Lable { get; set; }
    }

	public class OrderDetailDTO1
	{
		public int OrderId { get; set; }
		public DateTime? OrderDate { get; set; }
		public int? Status { get; set; }
		public decimal? TotalAmount { get; set; }
		public string? Note { get; set; }
		public int? Type { get; set; }
		public ICollection<OrderItemDTO1> OrderDetails { get; set; }
	}

	public class OrderItemDTO1
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
