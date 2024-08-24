namespace EHM_API.DTOs.OrderDTO.Manager
{
	public class CreateInvoiceForOrder2DTO
	{
		public DateTime? PaymentTime { get; set; }
		public decimal? PaymentAmount { get; set; }
		public string? Taxcode { get; set; }
		public int PaymentStatus { get; set; }

		public int? AccountId { get; set; }
		public decimal? AmountReceived { get; set; }
		public decimal? ReturnAmount { get; set; }
		public int? PaymentMethods { get; set; }

		public string? Description { get; set; }

	}
}
