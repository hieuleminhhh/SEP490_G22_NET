namespace EHM_API.DTOs.OrderDTO.Cashier
{
    namespace EHM_API.DTOs.OrderDTO
    {
        public class OrderDetailWithStaffDTO
        {
            public int OrderId { get; set; }
            public DateTime? OrderDate { get; set; }
            public int? Status { get; set; }
            public DateTime? RecevingOrder { get; set; }
            public decimal? TotalAmount { get; set; }
            public string? GuestPhone { get; set; }
            public decimal? Deposits { get; set; }
            public string? Note { get; set; }
            public string? CancelationReason { get; set; }
            public DateTime? ShipTime { get; set; }

            // Thông tin Account của khách hàng
            public string? AccountFirstName { get; set; }
            public string? AccountLastName { get; set; }

            // Thông tin Staff
            public string? StaffFirstName { get; set; }
            public string? StaffLastName { get; set; }

            // Thông tin chi tiết Address
            public AddressWithStaffDTO? Address { get; set; }

            // Thông tin Discount
            public DiscountWithStaffDTO? Discount { get; set; }

            // Thông tin chi tiết đơn hàng (OrderDetails)
            public List<OrderDetailStaffDTO> OrderDetails { get; set; }

            // Thông tin các đặt trước (Reservations)
            public List<ReservationWithStaffDTO> Reservations { get; set; }
        }

        public class AddressWithStaffDTO
        {
            public string? GuestAddress { get; set; }
            public string? ConsigneeName { get; set; }
            public string? GuestPhone { get; set; }
        }

        public class DiscountWithStaffDTO
        {
            public int DiscountId { get; set; }
            public decimal? DiscountAmount { get; set; }
        }

        public class OrderDetailStaffDTO
        {
            public int OrderDetailId { get; set; }
            public decimal? UnitPrice { get; set; }
            public int? Quantity { get; set; }
            public string? DishName { get; set; }
            public string? ComboName { get; set; }
            public string? Note { get; set; }
        }

        public class ReservationWithStaffDTO
        {
            public int ReservationId { get; set; }
            public DateTime? ReservationTime { get; set; }
            public int? GuestNumber { get; set; }
        }
    }

}
