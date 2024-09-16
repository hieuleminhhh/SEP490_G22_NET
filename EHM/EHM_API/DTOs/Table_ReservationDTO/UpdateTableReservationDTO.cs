namespace EHM_API.DTOs.Table_ReservationDTO
{
    namespace EHM_API.DTOs
    {
        public class UpdateTableReservationDTO
        {
            public int ReservationId { get; set; }
            public List<int> TableIds { get; set; }
        }
    }

}
