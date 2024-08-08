namespace EHM_API.DTOs.OrderDTO.Guest
{
    public class OrderDTOAll
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public int? InvoiceId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public decimal Deposits { get; set; }
        public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
        public string? Note { get; set; }
		public int? Type { get; set; }
		public int? DiscountId { get; set; }
		public int? DiscountPercent { get; set; }
		public string? DiscountName { get; set; }
		public int? QuantityLimit { get; set; }

		public decimal? AmountReceived { get; set; }
		public decimal? ReturnAmount { get; set; }
		public int? PaymentMethods { get; set; }

		public int PaymentStatus { get; set; }
		public DateTime? PaymentTime { get; set; }
		public string? Taxcode { get; set; }

		public IEnumerable<OrderDetailDTO> OrderDetails { get; set; }
		public IEnumerable<TableOfOrderDTO> Tables { get; set; }

	}
	public class TableOfOrderDTO
	{
		public int TableId { get; set; }
		public int? Status { get; set; }
		public int? Capacity { get; set; }
		public int? Floor { get; set; }
	}

}