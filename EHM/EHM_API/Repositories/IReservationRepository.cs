using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.ReservationDTO.Manager;
using EHM_API.Models;

namespace EHM_API.Repositories
{
	public interface IReservationRepository
	{
		Task<IEnumerable<Reservation>> GetReservationsByStatus(int? status);

		Task<CReservationDTO> CreateReservationAsync(CreateReservationDTO reservationDTO);
		Task<Guest> GetOrCreateGuest(string guestPhone, string email);
		Task<Address> GetAddressByGuestPhoneAsync(string guestPhone);
		Task<Address> GetOrCreateAddress(string guestPhone, string? guestAddress, string? consigneeName);
		Task<Reservation> GetReservationDetailAsync(int reservationId);
        Task UpdateReservationAsync(Reservation reservation);
        Task<bool> UpdateTableIdAsync(UpdateTableIdDTO updateTableIdDTO);

		Task<int> CountOrdersWithStatusOnDateAsync(DateTime date, int status);
		Task<int> GetTotalTablesAsync();

		Task<IEnumerable<Reservation>> SearchReservationsAsync(string? guestNameOrPhone);

		Task<Reservation?> GetReservationByIdAsync(int reservationId);
		Task<List<Table>> GetAllTablesAsync();

		Task<List<(Table, DateTime?)>> GetTablesWithCurrentDayReservationsAsync(int reservationId);
		Task<List<(Table, DateTime?)>> GetTablesByReservationIdAsync(int reservationId);
        Task<Reservation?> UpdateReasonCancelAsync(int reservationId, string? reasonCancel, string? cancelBy);

		Task<Reservation?> GetReservationByOrderIdAsync(int orderId);

		Task<Reservation> GetReservationsByTableIdAsync(int tableId);
        Task UpdateReservationOrderAsync(int reservationId, int orderId);

		IQueryable<Reservation> GetReservationsForTimeAndStatus(DateTime time, int status);
		IQueryable<Table> GetAvailableTables(DateTime reservationTime, int guestNumber);

		IQueryable<Reservation> GetReservationsForDateTime(DateTime reservationTime);

		IQueryable<int> GetReservedTableIdsForTime(DateTime reservationTime);
		IQueryable<Table> GetAllTables();
        Task SaveChangesAsync();
        void CreateNotification(Notification notification);
		Task<bool> UpdateReservationAcceptByAsync(UpdateReservationAcceptByDTO dto);
        void UpdateReservations(IEnumerable<Reservation> reservations);
        void UpdateOrders(IEnumerable<Order> orders);
    }
}
