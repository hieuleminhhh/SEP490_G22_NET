﻿namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderDetailForChef1DTO
    {
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public string? Note { get; set; }
        public int? DishesServed { get; set; }
        public virtual ICollection<ComboDetailForChefDTO> ComboDetailsForChef { get; set; }
    }
}