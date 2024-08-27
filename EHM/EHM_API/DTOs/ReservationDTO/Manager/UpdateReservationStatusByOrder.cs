namespace EHM_API.DTOs.ReservationDTO.Manager
{
    public class UpdateReservationStatusByOrder
    {
        public int OrderId { get; set; }
        public int Status { get; set; }
    }
}
