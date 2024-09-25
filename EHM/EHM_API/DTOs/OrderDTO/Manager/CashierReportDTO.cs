namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class CashierReportDTO
    {
        public int CashierId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ShipOrderCount { get; set; } // Số đơn ship
        public int DineInOrderCount { get; set; } // Số đơn hoàn thành tại quán
        public int TakeawayOrderCount { get; set; } // Số đơn mang về
        public int RefundOrderCount { get; set; } // Số đơn hoàn tiền
        public decimal Revenue { get; set; } // Doanh thu
        public decimal TotalRefunds { get; set; } // Số tiền đã hoàn
        public int CompletedOrderCount { get; set; }
    }

}
