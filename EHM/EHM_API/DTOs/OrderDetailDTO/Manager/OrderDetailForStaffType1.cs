namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderDetailForStaffType1
    {
        public int OrderId { get; set; }    
        public int? OrderType { get; set; }
        public int? Status { get; set; }
        public virtual ICollection<ItemInOrderDetail> ItemInOrderDetails { get; set; }
    }
}

