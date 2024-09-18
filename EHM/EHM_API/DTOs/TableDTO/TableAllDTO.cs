public class TableAllDTO
{
    public int TableId { get; set; }
    public int? Status { get; set; }
    public int? Capacity { get; set; }
    public string? Floor { get; set; }
    public string? Lable { get; set; }
    public List<TableReservationDetailDTO> TableReservations { get; set; } = new List<TableReservationDetailDTO>();
}

public class TableReservationDetailDTO
{
    public int ReservationId { get; set; }
    public DateTime? ReservationTime { get; set; }
    public int? GuestNumber { get; set; }
    public int? Status { get; set; }
    public string? ConsigneeName { get; set; }
    public string? GuestPhone { get; set; }
}
