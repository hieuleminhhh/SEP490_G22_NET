namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class ComboDetailForChefDTO
    {
        public string? ComboName { get; set; }
        public List<ItemInComboDTO>? ItemsInCombo { get; set; }
        public string? Note { get; set; }
        public DateTime? OrderTime { get; set; }
    }
}
