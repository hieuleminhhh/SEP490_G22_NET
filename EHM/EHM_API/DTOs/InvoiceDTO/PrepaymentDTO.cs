namespace EHM_API.DTOs.InvoiceDTO
{
    public class PrepaymentDTO
    {
		public decimal Deposits { get; set; }
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
