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
		public int? AccountId { get; set; }
		public decimal Deposits { get; set; }
		public string? Note { get; set; }
		public int? Type { get; set; }
		public int? DiscountId { get; set; }
		public List<CartOrderDetailsDTO> OrderDetails { get; set; }
	}
}
