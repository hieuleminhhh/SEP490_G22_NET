﻿using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.Models;

namespace EHM_API.Repositories
{
	public interface IReservationRepository
	{
		Task<IEnumerable<Reservation>> GetReservationsByStatus(int? status);

		Task CreateReservationAsync(Reservation reservation);
		Task<Guest> GetOrCreateGuest(string guestPhone, string email);
		Task<Address> GetAddressByGuestPhoneAsync(string guestPhone);
		Task<Address> GetOrCreateAddress(string guestPhone, string? guestAddress, string? consigneeName);
		Task<Reservation> GetReservationDetailAsync(int reservationId);
        Task UpdateReservationAsync(Reservation reservation);
        Task<bool> UpdateTableIdAsync(UpdateTableIdDTO updateTableIdDTO);

		Task<int> CountOrdersWithStatusOnDateAsync(DateTime date, int status);
		Task<int> GetTotalTablesAsync();

		Task<IEnumerable<Reservation>> SearchReservationsAsync(string? guestNameOrPhone);
	}
}
