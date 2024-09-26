namespace EHM_API.DTOs.CartDTO.OrderStaff
{
    public class GetRemainingItemsResponseDTO
    {
        public List<ComboRemainingDTO> Combos { get; set; }
        public List<DishRemainingDTO> Dishes { get; set; }
    }

    public class ComboRemainingDTO
    {
        public int ComboId { get; set; }
        public string Name { get; set; }
        public int? QuantityRemaining { get; set; }
        public bool IsServed { get; set; }
    }

    public class DishRemainingDTO
    {
        public int DishId { get; set; }
        public string Name { get; set; }
        public int? QuantityRemaining { get; set; }
        public bool IsServed { get; set; }
    }
}
