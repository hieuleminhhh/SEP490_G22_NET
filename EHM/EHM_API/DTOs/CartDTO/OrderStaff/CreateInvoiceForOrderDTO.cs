namespace EHM_API.DTOs.CartDTO.OrderStaff
{
	public class CreateInvoiceForOrderDTO
	{
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
