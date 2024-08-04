using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.Models;

namespace EHM_API.DTOs.DiscountDTO.Manager
{
    public class DiscountWithDishesDTO
    {
        public int DiscountId { get; set; }
        public string DiscountName { get; set; }
        public string Note { get; set; }
        public bool DiscountStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Type { get; set; }
        public int? DiscountPercent { get; set; }
        public int? QuantityLimit { get; set; }
        public IEnumerable<DishDTOAll> Dishes { get; set; }
    }
}
