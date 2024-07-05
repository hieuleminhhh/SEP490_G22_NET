using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		
		public async Task<Reservation> GetReservationDetailAsync(int reservationId)
		{
			return await _context.Reservations
				.Include(r => r.Address)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Dish)
							.ThenInclude(d => d.Discount)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Combo)
				.Include(r => r.Order)
					.ThenInclude(o => o.Address)
					.Include(r => r.TableReservations)
					.ThenInclude(tr => tr.Table)
				.FirstOrDefaultAsync(r => r.ReservationId == reservationId);
		}


		public async Task<bool> UpdateTableIdAsync(UpdateTableIdDTO updateTableIdDTO)
		{
			/*		var reservation = await _context.Reservations.FindAsync(updateTableIdDTO.ReservationId);
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
					}*/
			return false;
		}

		public async Task<Guest> GetOrCreateGuest(string guestPhone, string email)
		{
			var guest = await _context.Guests
				.FirstOrDefaultAsync(g => g.GuestPhone == guestPhone);

			if (guest == null)
			{
				guest = new Guest
				{
					GuestPhone = guestPhone,
					Email = email
				};

				await _context.Guests.AddAsync(guest);
				await _context.SaveChangesAsync();
			}

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

		public async Task<Address> GetAddressByGuestPhoneAsync(string guestPhone)
		{
			return await _context.Addresses.FirstOrDefaultAsync(a => a.GuestPhone == guestPhone);
		}


		public async Task<IEnumerable<Reservation>> GetReservationsByStatus(int? status)
		{
			return await _context.Reservations
				.Include(r => r.Address)
					.ThenInclude(a => a.GuestPhoneNavigation)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Dish)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Combo)
				.Include(r => r.Order)
					.ThenInclude(o => o.Address)
				.Include(r => r.TableReservations)
					.ThenInclude(tr => tr.Table)
				.Where(r => !status.HasValue || r.Status == status)
				.ToListAsync();
		}


		public async Task<int> CountOrdersWithStatusOnDateAsync(DateTime date, int status)
		{
			return await _context.Reservations
				.CountAsync(o => o.ReservationTime.Value.Date == date.Date && o.Status == status);
		}

		public async Task<int> GetTotalTablesAsync()
		{
			return await _context.Tables.CountAsync();
		}
        public async Task UpdateReservationAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);

            if (reservation.Order != null)
            {
                _context.Orders.Update(reservation.Order);
            }

            await _context.SaveChangesAsync();
        }

		public async Task<IEnumerable<Reservation>> SearchReservationsAsync(string? guestNameOrPhone)
		{
			var query = _context.Reservations.AsQueryable();

			if (!string.IsNullOrWhiteSpace(guestNameOrPhone))
			{
				var searchValue = guestNameOrPhone.ToLower();

				query = query.Where(r =>
					EF.Functions.Like(EF.Functions.Collate(r.Address.GuestPhone, "SQL_Latin1_General_CP1_CI_AS"), $"%{searchValue}%") ||
					EF.Functions.Like(EF.Functions.Collate(r.Address.ConsigneeName, "SQL_Latin1_General_CP1_CI_AS"), $"%{searchValue}%")
				);
			}

			return await query
				.Include(r => r.Address)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Dish)
				.Include(r => r.Order)
					.ThenInclude(o => o.OrderDetails)
						.ThenInclude(od => od.Combo)
				.Include(r => r.TableReservations)
					.ThenInclude(tr => tr.Table)
				.ToListAsync();
		}




	}
}
