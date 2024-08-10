namespace EHM_API.DTOs.CartDTO.OrderStaff
{
	public class InvoiceDetailDTO
	{
		public int InvoiceId { get; set; }
		public decimal? PaymentAmount { get; set; }
		public DateTime? PaymentTime { get; set; }
		public int PaymentStatus { get; set; }

		public string? Address { get; set; }
        public string? ConsigneeName { get; set; }
		public string? GuestPhone { get; set; }
		public DateTime? OrderDate { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? AmountReceived { get; set; }
		public decimal? ReturnAmount { get; set; }
		public string? Taxcode { get; set; }
        public string DiscountName { get; set; }
        public int? DiscountPercent { get; set; }
        public string Note { get; set; }
        public decimal? TotalMoney { get; set; }
        public int? QuantityLimit { get; set; }

		public decimal Deposits { get; set; }
		public IEnumerable<ItemInvoiceDTO> ItemInvoice { get; set; }

	}
	public class ItemInvoiceDTO
	{
		public int DishId { get; set; }
		public string? ItemName { get; set; }
		public int ComboId { get; set; }
		public string? NameCombo { get; set; }
		public decimal? Price { get; set; }
		public decimal? UnitPrice { get; set; }
		public int? Quantity { get; set; }
	}
}
