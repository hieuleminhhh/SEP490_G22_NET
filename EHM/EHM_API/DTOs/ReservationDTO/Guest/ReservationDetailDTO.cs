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
		public int? TableId { get; set; }
		public string? GuestPhone { get; set; }
		public OrderDetailDTO1 Order { get; set; }
		public GuestDTO1 Guest { get; set; }


	}
	public class OrderDetailDTO1
	{
		public int OrderId { get; set; }
		public DateTime? OrderDate { get; set; }
		public int? Status { get; set; }
		public decimal? TotalAmount { get; set; }
		public string? Note { get; set; }
		public ICollection<OrderItemDTO1> OrderDetails { get; set; }
		public AddressDTO1 Address { get; set; }
	}

	public class OrderItemDTO1
	{
		public int DishId { get; set; }
		public string? ItemName { get; set; }
		public int ComboId { get; set; }
		public string? NameCombo { get; set; }

		public decimal? UnitPrice { get; set; }
		public int? Quantity { get; set; }
	}

	public class GuestDTO1
	{
		public string GuestPhone { get; set; }
		public string? Email { get; set; }
	}

	public class AddressDTO1
	{
		public int AddressId { get; set; }
		public string? GuestAddress { get; set; }
		public string? ConsigneeName { get; set; }
		public string? GuestPhone { get; set; }
	}

}
