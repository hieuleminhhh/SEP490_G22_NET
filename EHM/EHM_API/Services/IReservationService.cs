using EHM_API.DTOs.OrderDTO.Guest;
using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.ReservationDTO.Manager;
using EHM_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IReservationService
	{
		Task<IEnumerable<ReservationByStatus>> GetReservationsByStatus(int? status);
		Task<ReservationDetailDTO> GetReservationDetailAsync(int reservationId);
        Task<CReservationDTO> CreateReservationAsync(CreateReservationDTO reservationDTO);

        Task UpdateStatusAsync(int reservationId, UpdateStatusReservationDTO updateStatusReservationDTO);
        Task<int?> CalculateStatusOfTable(Reservation reservation);
        Task RegisterTablesAsync(RegisterTablesDTO registerTablesDTO);
		Task<IEnumerable<ReservationSearchDTO>> SearchReservationsAsync(string? guestNameOrPhone);

		Task<CheckTimeReservationDTO?> GetReservationTimeAsync(int reservationId);
		Task UpdateReservationAndTableStatusAsync(int reservationId, int tableId, int reservationStatus, int tableStatus);
        Task<bool> UpdateTableStatusesAsync(int reservationId, int newStatus);
        Task<ReasonCancelDTO?> UpdateReasonCancelAsync(int reservationId, ReasonCancelDTO? reasonCancelDTO);

		Task<GetReservationByOrderDTO?> GetReservationByOrderIdAsync(int orderId);

		Task<GetReservationByOrderDTO> GetReservationsByTableIdAsync(int tableId);
        Task<bool> UpdateReservationOrderAsync(UpdateReservationOrderDTO dto);
        Task<UpdateReservationStatusByOrder?> UpdateReservationStatusAsync(UpdateReservationStatusByOrder updateReservationStatusByOrder);

		ReservationCheckResult CheckReservation(DateTime reservationTime, int guestNumber);
		Task<bool> UpdateReservationAcceptByAsync(UpdateReservationAcceptByDTO dto);

    }
}