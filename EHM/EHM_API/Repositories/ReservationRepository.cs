using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
	public class ReservationRepository : IReservationRepository
	{
		private readonly EHMDBContext _context;

		public ReservationRepository(EHMDBContext context)
		{
			_context = context;
		}

		public async Task<bool> UpdateStatusAsync(UpdateStatusReservationDTO updateStatusDto)
		{
			var reservation = await _context.Reservations.FindAsync(updateStatusDto.ReservationId);
			if (reservation == null) return false;

			reservation.Status = updateStatusDto.Status;
			_context.Entry(reservation).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
				return true;
			}
			catch (DbUpdateConcurrencyException)
			{
				return false;
			}
		}

		public async Task<Reservation> GetReservationDetailAsync(int reservationId)
		{
			return await _context.Reservations
				.Include(r => r.GuestPhoneNavigation)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Dish)
							.ThenInclude(d => d.Discount)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Combo)
				.Include(r => r.Order)
					.ThenInclude(o => o.Address)
				.FirstOrDefaultAsync(r => r.ReservationId == reservationId);
		}


		public async Task<bool> UpdateTableIdAsync(UpdateTableIdDTO updateTableIdDTO)
		{
			var reservation = await _context.Reservations.FindAsync(updateTableIdDTO.ReservationId);
			if (reservation == null) return false;

			reservation.TableId = updateTableIdDTO.TableId;
			_context.Entry(reservation).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
				return true;
			}
			catch (DbUpdateConcurrencyException)
			{
				return false;
			}
		}

		public async Task<Guest> GetOrCreateGuest(string guestPhone, string email)
		{
			var guest = await _context.Guests
				.FirstOrDefaultAsync(g => g.GuestPhone == guestPhone);

			if (guest != null)
			{
				guest.Email = email;
			}
			else
			{
				guest = new Guest
				{
					GuestPhone = guestPhone,
					Email = email
				};
				await _context.Guests.AddAsync(guest);
			}

			await _context.SaveChangesAsync();
			return guest;
		}

		public async Task<Address> GetOrCreateAddress(string guestPhone, string guestAddress, string consigneeName)
		{
			var address = await _context.Addresses
				.FirstOrDefaultAsync(a =>
					a.GuestPhone == guestPhone &&
					a.GuestAddress == guestAddress &&
					a.ConsigneeName == consigneeName);

			if (address == null)
			{
				// Create new address
				address = new Address
				{
					GuestPhone = guestPhone,
					GuestAddress = guestAddress,
					ConsigneeName = consigneeName
				};
				await _context.Addresses.AddAsync(address);
				await _context.SaveChangesAsync();
			}

			return address;
		}


		public async Task CreateReservationAsync(Reservation reservation)
		{
			_context.Reservations.Add(reservation);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Reservation>> GetReservationsByStatus(int? status)
		{
			return await _context.Reservations
				.Include(r => r.GuestPhoneNavigation)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Dish)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Combo)
				.Include(r => r.Order)
					.ThenInclude(o => o.Address)
				.Include(r => r.Table)
				.Where(r => !status.HasValue || r.Status == status)
				.ToListAsync();
		}


	}
}
