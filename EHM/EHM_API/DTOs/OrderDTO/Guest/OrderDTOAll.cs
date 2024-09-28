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
        public string? EmailOfGuest { get; set; }
        public int AccountId { get; set; }
        public string? EmailOfAccount { get; set; }
        public decimal Deposits { get; set; }
        public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
        public string? Note { get; set; }
		public int? Type { get; set; }
		public int? DiscountId { get; set; }
		public string? CancelationReason { get; set; }
		public int? DiscountPercent { get; set; }
		public string? DiscountName { get; set; }
		public int? QuantityLimit { get; set; }

		public decimal? AmountReceived { get; set; }
		public decimal? ReturnAmount { get; set; }
		public int? PaymentMethods { get; set; }

		public int PaymentStatus { get; set; }
		public DateTime? PaymentTime { get; set; }
		public string? Taxcode { get; set; }
        public int? ShipId { get; set; }
        public string? CancelBy { get; set; }
        public DateTime? ShipTime { get; set; }
        public int? StaffId { get; set; }
        public string? StaffFirstName { get; set; }
        public string? StaffLastName { get; set; }
        public int? AcceptBy { get; set; }
        public string? AcceptByFirstName { get; set; }
        public string? AcceptByLastName { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime? RefundDate { get; set; }
        public IEnumerable<OrderDetailDTO> OrderDetails { get; set; }
        public IEnumerable<TableOfOrderDTO> Tables { get; set; }
        public ReservationDTOByOrderId? Reservation { get; set; }

    }
	public class TableOfOrderDTO
	{
        public int TableId { get; set; }
        public int? Status { get; set; }
        public int? Capacity { get; set; }
        public string? Floor { get; set; }
        public string? Lable { get; set; }
    }
    public class ReservationDTOByOrderId
    {
        public int ReservationId { get; set; }
        public DateTime? ReservationTime { get; set; }
        public int? GuestNumber { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }
        public string? ReasonCancel { get; set; }
        public DateTime? TimeIn { get; set; }
        public IEnumerable<TableOfReservationDTO> TablesOfReservation { get; set; }
    }
    public class TableOfReservationDTO
    {
        public int TableId { get; set; }
        public int? Status { get; set; }
        public int? Capacity { get; set; }
        public string? Floor { get; set; }
        public string? Lable { get; set; }
    }
}