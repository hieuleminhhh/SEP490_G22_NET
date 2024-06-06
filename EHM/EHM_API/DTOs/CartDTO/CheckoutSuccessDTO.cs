namespace EHM_API.DTOs.CartDTO
{
    public class CheckoutSuccessDTO
    {    
            public string GuestAddress { get; set; }
            public string ConsigneeName { get; set; }
            public string GuestPhone { get; set; }
            public DateTime? ReceivingTime { get; set; }
            public string Email { get; set; }
            public List<OrderDetailDTO> OrderDetails { get; set; }
            public List<ComboDetailDTO> ComboDetails { get; set; }
        }

        public class OrderDetailDTO
        {
            public int DishId { get; set; }
            public string ItemName { get; set; }
            public string ItemDescription { get; set; }
            public decimal? Price { get; set; }
            public string ImageUrl { get; set; }
            public string? CategoryName { get; set; }
            public int? DiscountId { get; set; }
        }
        public class ComboDetailDTO
        {
            public int ComboId { get; set; }
            public string? NameCombo { get; set; }
            public decimal? Price { get; set; }
            public string? Note { get; set; }
            public string? ImageUrl { get; set; }
        }
    }

