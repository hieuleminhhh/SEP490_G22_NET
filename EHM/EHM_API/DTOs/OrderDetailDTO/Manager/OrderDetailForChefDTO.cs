namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderDetailForChefDTO
    {
        public string ItemName { get; set; }
        public string ComboName { get; set; }
        public string ItemInComboName { get; set; }
        public int Quantity { get; set; }
        public int? DishesServed { get; set; }
    }
}
