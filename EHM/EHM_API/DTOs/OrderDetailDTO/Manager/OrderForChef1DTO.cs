namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderForChef1DTO
    {
        public int OrderId { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public int? Status { get; set; }
        public int? Type { get; set; }
        public IEnumerable<OrderDetailForChefDTO> OrderDetails { get; set; }
    }
}
