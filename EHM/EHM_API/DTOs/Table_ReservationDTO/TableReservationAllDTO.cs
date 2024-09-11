namespace EHM_API.DTOs.Table_ReservationDTO
{
    public class TableReservationAllDTO
    {
        public int? TableId { get; set; }
        public int ReservationId { get; set; }
        public DateTime? ReservationTime { get; set; }
        public int? GuestNumber { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }
        public string? ReasonCancel { get; set; }

        // Thông tin từ Address
        public string? ConsigneeName { get; set; }
        public string? GuestPhone { get; set; }
    }
}
