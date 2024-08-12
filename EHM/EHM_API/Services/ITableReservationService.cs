namespace EHM_API.Services
{
    public interface ITableReservationService
    {
        Task<bool> DeleteTableReservationByReservationIdAsync(int reservationId);
    }
}
