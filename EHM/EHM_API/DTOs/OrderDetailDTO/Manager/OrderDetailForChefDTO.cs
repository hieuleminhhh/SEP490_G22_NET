using EHM_API.Models;

namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderDetailForChefDTO
    {
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public DateTime? OrderDishDate { get; set; }
        public string? Note { get; set; }
        public int? DishesServed { get; set; }
        public virtual ICollection<ComboDetailForChefDTO> ComboDetailsForChef { get; set; }
    }
}
