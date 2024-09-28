using EHM_API.DTOs.OrderDetailDTO.Manager;

namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class OrderResponseDTO
    {
        public IEnumerable<OrderDetailForStaffType1> OrderDetails { get; set; }
        public decimal TotalPaymentAmount { get; set; }
    }
}
