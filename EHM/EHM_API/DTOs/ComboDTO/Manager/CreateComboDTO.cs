namespace EHM_API.DTOs.ComboDTO.Manager
{
    public class CreateComboDTO
    {
        public string? NameCombo { get; set; }
        public decimal? Price { get; set; }
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsActive { get; set; }
        public int? QuantityCombo { get; set; }
    }
}
