namespace EHM_API.DTOs.OrderDTO.Guest
{
      public class OrderDetailDTO
        {
            public int OrderDetailId { get; set; }
            public int DishId { get; set; }
            public int ComboId { get; set; }
            public string? NameCombo { get; set; }
            public string? ItemName { get; set; }
            public decimal? UnitPrice { get; set; }
            public decimal? Price { get; set; }
            public decimal? DiscountedPrice { get; set; }
            public int? Quantity { get; set; }
            public string? ImageUrl { get; set; }
            public int? DishesServed { get; set; }
    }
}

