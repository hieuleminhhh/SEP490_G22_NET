namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class ExportOrderDTO
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

        public ExportInvoiceDTO? Invoice { get; set; }
        public ExportAddressDTO? Address { get; set; }
        public ExportGuestDTO? Guest { get; set; }
        public List<ExportOrderDetailDTO>? OrderDetails { get; set; }
        public List<ExportReservationDTO>? Reservations { get; set; }
    }

    public class ExportInvoiceDTO
    {
        public int InvoiceId { get; set; }
        public DateTime? PaymentTime { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string? Taxcode { get; set; }
        public int PaymentStatus { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class ExportAddressDTO
    {
        public int AddressId { get; set; }
        public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
        public string? GuestPhone { get; set; }
    }

    public class ExportGuestDTO
    {
        public string GuestPhone { get; set; } = null!;
        public string? Email { get; set; }
    }

    public class ExportOrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public string? DishName { get; set; }
        public int? DishesServed { get; set; }
    }

    public class ExportReservationDTO
    {
        public int ReservationId { get; set; }
        public DateTime? ReservationTime { get; set; }
        public int? GuestNumber { get; set; }
        public string? Note { get; set; }
        public int AddressId { get; set; }
        public string? ReasonCancel { get; set; }
        public List<int>? TableIds { get; set; }
    }
}