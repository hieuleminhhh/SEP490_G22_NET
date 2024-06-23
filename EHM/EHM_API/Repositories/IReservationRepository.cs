﻿using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.Models;

namespace EHM_API.Repositories
{
	public interface IReservationRepository
	{
		Task CreateReservationAsync(Reservation reservation);
		Task<Guest> GetOrCreateGuest(string guestPhone, string email);
		Task<Address> GetOrCreateAddress(string guestPhone, string? guestAddress, string? consigneeName);
		Task<IEnumerable<Reservation>> GetReservationsByStatusAsync(int? status);
		Task<Reservation> GetReservationDetailAsync(int reservationId);
		Task<bool> UpdateStatusAsync(UpdateStatusReservationDTO updateStatusDto);
		Task<bool> UpdateTableIdAsync(UpdateTableIdDTO updateTableIdDTO);
	}
}