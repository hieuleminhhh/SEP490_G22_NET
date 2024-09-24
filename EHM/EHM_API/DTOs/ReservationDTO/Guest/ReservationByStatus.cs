using EHM_API.Models;

namespace EHM_API.DTOs.ReservationDTO.Guest
{
	public class ReservationByStatus
	{
		public int ReservationId { get; set; }

		public string? ConsigneeName { get; set; }
		public string? GuestPhone { get; set; }

		public string? Email { get; set; }
		public DateTime? ReservationTime { get; set; }
		public int? GuestNumber { get; set; }
		public string? GuestAddress { get; set; }
		public decimal Deposits { get; set; }
		public string? Note { get; set; }
		public int? Status { get; set; }
        public int? AccountId { get; set; }
        public string? CancelBy { get; set; }
        public int? AcceptBy { get; set; }
        public OrderDetailDTO3 Order { get; set; }
		public ICollection<TableReservationDTO> TableOfReservation { get; set; }

	}

	public class TableReservationDTO
	{
		public int? TableId { get; set; }
		public int? Capacity { get; set; }
		public string? Floor { get; set; }
        public string? Lable { get; set; }
    }

	public class OrderDetailDTO3
	{
		public int OrderId { get; set; }
		public DateTime? OrderDate { get; set; }
		public int? Status { get; set; }
		public decimal? TotalAmount { get; set; }
		public string? Note { get; set; }
        public string? CancelBy { get; set; }
        public int? AccountId { get; set; }
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
