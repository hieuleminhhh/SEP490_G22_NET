using EHM_API.Models;

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
        public decimal TotalCashToSubmit { get; set; }
        public List<OrderReportDTO> ListOrder { get; set; }
    }
    public class OrderReportDTO
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public decimal? Deposits { get; set; }
        public string? Note { get; set; }
        public int? Type { get; set; }
        public bool IsRefundOrder { get; set; } // Thêm thuộc tính để phân biệt đơn hoàn tiền
    }
}
