namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderDetailForStaffType1
    {
        public int OrderId { get; set; }    
        public int? OrderType { get; set; }
        public int? Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public string? GuestPhone { get; set; }
        public decimal Deposits { get; set; }
        public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
        public virtual ICollection<ItemInOrderDetail> ItemInOrderDetails { get; set; }
    }
}

