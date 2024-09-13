namespace EHM_API.DTOs.InvoiceDTO
{
	public class UpdatePaymentStatusDTO
	{
		public decimal PaymentAmount { get; set; }
        public int CollectedBy { get; set; }
    }
}