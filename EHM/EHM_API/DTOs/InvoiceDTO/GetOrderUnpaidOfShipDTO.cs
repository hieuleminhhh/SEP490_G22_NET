namespace EHM_API.DTOs.InvoiceDTO
{
	public class GetOrderUnpaidOfShipDTO
	{
		public int OrderId { get; set; }
		public DateTime? OrderDate { get; set; }
		public int? Status { get; set; }
		public DateTime? RecevingOrder { get; set; }
        public int? StaffId { get; set; }
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }
        public int? InvoiceId { get; set; }
		public decimal? TotalAmount { get; set; }
        public decimal? PaymentStatus { get; set; }
        public string? GuestPhone { get; set; }
		public decimal? Deposits { get; set; }
		public int? AddressId { get; set; }
		public string? Note { get; set; }
		public int? Type { get; set; }
		public int? DiscountId { get; set; }
		public string? CancelationReason { get; set; }
        public decimal? TotalPaid { get; set; }
        public string? CollectedFirstName { get; set; }
        public string? CollectedLastName { get; set; }
    }
}
