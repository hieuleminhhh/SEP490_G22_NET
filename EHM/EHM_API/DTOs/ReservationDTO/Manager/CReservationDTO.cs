namespace EHM_API.DTOs.ReservationDTO.Manager
{


    public class CReservationDTO
{
        public int ReservationId { get; set; }
        public int? AccountId { get; set; }
        public string GuestPhone { get; set; } = null!;
        public string? Email { get; set; }
        public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
        public DateTime? ReservationTime { get; set; }
        public int? GuestNumber { get; set; }
        public string? Note { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal Deposits { get; set; }
        public int? Type { get; set; }


        public List<CReservationOrderDetailsDTO>? OrderDetails { get; set; }
    }

    public class CReservationOrderDetailsDTO
    {
        public int? DishId { get; set; }
        public int? ComboId { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Note { get; set; }
        public DateTime? OrderTime { get; set; }
    }
}
