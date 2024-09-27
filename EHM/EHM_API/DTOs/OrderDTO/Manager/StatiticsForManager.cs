namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class StatiticsForManager
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public int? AccountId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public decimal? Deposits { get; set; }
        public int? AddressId { get; set; }
        public string? Note { get; set; }
        public int? Type { get; set; }
        public int? DiscountId { get; set; }
        public string? CancelationReason { get; set; }
        public int? StaffId { get; set; }
        public string? CancelBy { get; set; }
        public DateTime? ShipTime { get; set; }
        public int? CollectedBy { get; set; }
        public int? AcceptBy { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime? RefundDate { get; set; }

        // Thông tin Cashier
        public int CashierId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
