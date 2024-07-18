using EHM_API.DTOs.CartDTO.Guest;

namespace EHM_API.DTOs.CartDTO.OrderStaff
{
	public class TakeOutDTO
	{
				public string? GuestPhone { get; set; } = null!;
				public string? Email { get; set; }
				public int? AddressId { get; set; }
				public string? GuestAddress { get; set; }
				public string? ConsigneeName { get; set; }
				public DateTime? OrderDate { get; set; }
				public int? Status { get; set; }
				public DateTime? RecevingOrder { get; set; }
				public decimal? TotalAmount { get; set; }
				public decimal Deposits { get; set; }
				public string? Note { get; set; }
				public int? Type { get; set; }
				public List<CartOrderDetailsDTO> OrderDetails { get; set; }

		public DateTime? PaymentTime { get; set; }
		public decimal? PaymentAmount { get; set; }
		public int? DiscountId { get; set; }
		public string? Taxcode { get; set; }
		public int PaymentStatus { get; set; }
		public decimal? AmountReceived { get; set; }
		public decimal? ReturnAmount { get; set; }
		public int? PaymentMethods { get; set; }

		public string? Description { get; set; }
	}
}
