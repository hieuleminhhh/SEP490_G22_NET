using EHM_API.DTOs.OrderDetailDTO.Manager;

namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class OrderResponseDTO
    {
        public OrderDetailForStaffType1 OrderDetail { get; set; }
        public decimal TotalPaymentAmount { get; set; }
        public decimal TotalPaymentAmount1 { get; set; }
    }
}
