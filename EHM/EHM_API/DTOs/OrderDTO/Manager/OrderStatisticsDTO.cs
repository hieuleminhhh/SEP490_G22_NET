namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class OrderStatisticsDTO
    {
        public int CollectedById { get; set; }
        public string? CollectedByFirstName { get; set; }
        public string? CollectedByLastName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueByPaymentMethod0 { get; set; }
        public decimal RevenueByPaymentMethod1 { get; set; }
        public decimal RevenueByPaymentMethod2 { get; set; }
        public int OrderCountByPaymentMethod0 { get; set; }
        public int OrderCountByPaymentMethod1 { get; set; }
        public int OrderCountByPaymentMethod2 { get; set; }
        public List<int> OrderIds { get; set; }
    }
}
