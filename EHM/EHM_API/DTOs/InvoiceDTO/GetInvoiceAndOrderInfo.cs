namespace EHM_API.DTOs.InvoiceDTO
{
	public class GetInvoiceAndOrderInfo
	{
		public int OrderId { get; set; }
		public int? InvoiceId { get; set; }

		public DateTime? PaymentTime { get; set; }
		public decimal? PaymentAmount { get; set; }
		public string? Taxcode { get; set; }
		public int PaymentStatus { get; set; }
		public string? CustomerName { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
		public decimal? AmountReceived { get; set; }
		public decimal? ReturnAmount { get; set; }
		public int? PaymentMethods { get; set; }



	}
}
