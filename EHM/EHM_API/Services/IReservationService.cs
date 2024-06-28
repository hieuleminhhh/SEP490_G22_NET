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
		Task CreateReservationAsync(CreateReservationDTO createDto);
        Task UpdateStatusAsync(int reservationId, UpdateStatusReservationDTO updateStatusReservationDTO);
        Task<int?> CalculateStatusOfTable(Reservation reservation);
        Task RegisterTablesAsync(RegisterTablesDTO registerTablesDTO);

    }
}
