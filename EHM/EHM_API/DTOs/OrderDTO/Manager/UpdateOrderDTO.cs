namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class UpdateOrderDTO
    {
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public int? AccountId { get; set; }
        public int? TableId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public decimal Deposits { get; set; }
        public int AddressId { get; set; }
        public string? Note { get; set; }
		public int? Type { get; set; }
		public int? DiscountId { get; set; }
	}
}
