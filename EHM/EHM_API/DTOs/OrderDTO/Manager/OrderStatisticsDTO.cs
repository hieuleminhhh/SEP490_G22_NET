namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class OrderStatisticsDTO
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueByPaymentMethod1 { get; set; }
        public decimal RevenueByPaymentMethod2 { get; set; }
    }
}
