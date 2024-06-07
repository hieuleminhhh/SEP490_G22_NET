namespace EHM_API.DTOs.CartDTO
{
	public class CheckoutSuccessDTO
	{
		public string GuestPhone { get; set; }
		public string Email { get; set; }
		public int AddressId { get; set; }
		public string GuestAddress { get; set; }
		public string ConsigneeName { get; set; }
		public DateTime? OrderDate { get; set; }
		public int Status { get; set; }
		public DateTime? ReceivingTime { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal Deposits { get; set; }
		public List<OrderDetailDTO> OrderDetails { get; set; }
	}

	public class OrderDetailDTO
	{
		public string? NameCombo { get; set; }
		public string? ItemName { get; set; }
		public decimal? UnitPrice { get; set; }
		public int? Quantity { get; set; }
		public string? Note { get; set; }
		public string ImageUrl { get; set; }
		
	}


}
