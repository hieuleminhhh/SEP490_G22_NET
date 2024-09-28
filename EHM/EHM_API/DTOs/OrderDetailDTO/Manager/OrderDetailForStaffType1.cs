namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderDetailForStaffType1
    {
        public int OrderId { get; set; }    
        public int? OrderType { get; set; }
        public int? Status { get; set; }
        public int? StatusOrder { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string? GuestPhone { get; set; }
        public decimal Deposits { get; set; }
        public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
        public int? AccountId { get; set; }
        public string? CancelBy { get; set; }
        public string? CancelationReason { get; set; }
        public DateTime? CancelDate { get; set; }
        public int? StaffId { get; set; }
        public DateTime? ShipTime { get; set; }
        public int? CollectedBy { get; set; }
        public int? AcceptBy { get; set; }
        public DateTime? RefundDate { get; set; }
        public bool IsCollected { get; set; }
        public virtual ICollection<ItemInOrderDetail> ItemInOrderDetails { get; set; }
    }
}

