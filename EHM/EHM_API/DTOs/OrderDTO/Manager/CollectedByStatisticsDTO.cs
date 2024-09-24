namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class CollectedByStatisticsDTO
    {
        public int CollectedById { get; set; }
        public string? CollectedByFirstName { get; set; }
        public string? CollectedByLastName { get; set; }
        public List<OrderStatisticsDTO> Revenue { get; set; }  
    }

}
