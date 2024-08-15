namespace EHM_API.DTOs.DishDTO.Manager
{
    public class UpdateDishDTO
    {
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
    }
}
