namespace EHM_API.DTOs.CartDTO.OrderStaff
{
    public class GetRemainingItemsRequestDTO
    {
        public List<int> ComboIds { get; set; }
        public List<int> DishIds { get; set; }
    }
}
