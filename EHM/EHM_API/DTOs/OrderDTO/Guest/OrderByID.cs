namespace EHM_API.DTOs.OrderDTO.Guest
{
    public class OrdersByAccountDTO
    {
        public int AccountId { get; set; }
        public List<OrderByID> Orders { get; set; }
    }
    public class OrderByID
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public string? Note { get; set; }
        public AddressDTO1 Address { get; set; } 
        public List<OrderDetailDTO2> OrderDetails { get; set; } 
    }

 
    public class OrderDetailDTO2
    {
        public int OrderDetailId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public string? DishName { get; set; } 
        public string? ComboName { get; set; } 
        public string? Note { get; set; } 
    }


    public class AddressDTO1
    {
        public string? GuestAddress { get; set; } 
        public string? ConsigneeName { get; set; } 
        public string? GuestPhone { get; set; } 
    }
}
