namespace EHM_API.DTOs.InvoiceDTO
{
	public class InvoiceOfSitting
	{
		public int? Status { get; set; }

		public DateTime? PaymentTime { get; set; }
		public decimal? PaymentAmount { get; set; }
		public string? Taxcode { get; set; }
		public int PaymentStatus { get; set; }
		public string? CustomerName { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
		public int? AccountId { get; set; }
		public decimal? AmountReceived { get; set; }
		public decimal? ReturnAmount { get; set; }
		public int? PaymentMethods { get; set; }
		public string? Description { get; set; }
		public int? TableStatus { get; set; }
	}
}
