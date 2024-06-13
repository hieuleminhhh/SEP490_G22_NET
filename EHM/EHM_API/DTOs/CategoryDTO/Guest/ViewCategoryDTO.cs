namespace EHM_API.DTOs.CategoryDTO.Guest
{
    public class ViewCategoryDTO
    {
        public int DishId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }

    }
}
