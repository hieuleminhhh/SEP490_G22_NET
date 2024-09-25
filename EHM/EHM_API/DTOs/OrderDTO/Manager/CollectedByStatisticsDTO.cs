namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class CollectedByStatisticsDTO
    {
        public int CollectedById { get; set; }
        public string CollectedByFirstName { get; set; }
        public string CollectedByLastName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalRefundedAmount { get; set; }
        public decimal TotalCancelledOrdersRevenue { get; set; }
        public List<int> PaidOrderIds { get; set; }
        public List<int> RefundedOrderIds { get; set; }
        public List<int> CompletedOrderIds { get; set; }
        public List<int> UnreceivedDeliveryOrderIds { get; set; }
        public List<int> OverdueReservationOrderIds { get; set; }
        public List<int> UncollectedTakeawayOrderIds { get; set; }
    }
}
