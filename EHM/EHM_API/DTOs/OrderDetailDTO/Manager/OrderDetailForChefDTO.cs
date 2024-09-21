using EHM_API.Models;

namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderDetailForChefDTO
    {
        public int OrderDetailId { get; set; }
        public int? Type { get; set; }
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public DateTime? OrderTime { get; set; }
        public string? Note { get; set; }
        public int? DishesServed { get; set; }
        public int? OrderId { get; set; }
        public virtual ICollection<ComboDetailForChefDTO> ComboDetailsForChef { get; set; }
    }
}
