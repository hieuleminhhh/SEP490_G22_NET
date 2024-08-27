namespace EHM_API.DTOs.InvoiceDTO
{
    public class UpdateAmountInvoiceDTO
    {
        public int InvoiceId { get; set; }
        public decimal? PaymentAmount { get; set; }
        public decimal? ReturnAmount { get; set; }
    }
}
