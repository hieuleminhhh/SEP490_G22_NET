namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderForChefDTO
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public int? Type { get; set; }
        public IEnumerable<OrderDetailForChefDTO> OrderDetails { get; set; }
    }
}
