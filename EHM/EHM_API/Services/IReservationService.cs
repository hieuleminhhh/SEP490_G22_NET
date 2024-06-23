using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
	public interface IReservationService
	{
		Task<IEnumerable<ReservationRequestDTO>> GetReservationsByStatusAsync(int? status);
		Task<bool> UpdateStatusAsync(UpdateStatusReservationDTO updateStatusDto);
		Task<ReservationDetailDTO> GetReservationDetailAsync(int reservationId);
		Task<bool> UpdateTableIdAsync(UpdateTableIdDTO updateTableIdDTO);
		Task CreateReservationAsync(CreateReservationDTO createDto);
	}
}
