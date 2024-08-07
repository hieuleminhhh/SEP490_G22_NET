﻿using EHM_API.DTOs.ReservationDTO.Guest;
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
		private readonly IDishRepository _Dishrepository;
		private readonly IComboRepository _Comborepository;

		public ReservationRepository(EHMDBContext context, IDishRepository dishrepository,
			IComboRepository comborepository)
		{
			_context = context;
			_Dishrepository = dishrepository;
			_Comborepository = comborepository;
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

		public async Task<Reservation> CreateReservationAsync(CreateReservationDTO reservationDTO)
		{
			var guest = await GetOrCreateGuest(reservationDTO.GuestPhone, reservationDTO.Email);

			var address = await GetOrCreateAddress(
				reservationDTO.GuestPhone,
				reservationDTO.GuestAddress,
				reservationDTO.ConsigneeName
			);

			var reservation = new Reservation
			{
				AddressId = address.AddressId,
				ReservationTime = reservationDTO.ReservationTime,
				GuestNumber = reservationDTO.GuestNumber,
				Note = reservationDTO.Note,
				Status = reservationDTO.Status ?? 0
			};

			if (reservationDTO.OrderDetails != null)
			{
				var orderDetails = new List<OrderDetail>();

				foreach (var item in reservationDTO.OrderDetails)
				{
					Dish dish = null;
					Combo combo = null;

					if (item.DishId.HasValue && item.DishId > 0)
					{
						dish = await _Dishrepository.GetDishByIdAsync(item.DishId.Value);
						if (dish == null)
						{
							throw new KeyNotFoundException("Món ăn này không tồn tại");
						}
					}
					else if (item.ComboId.HasValue && item.ComboId > 0)
					{
						combo = await _Comborepository.GetComboByIdAsync(item.ComboId.Value);
						if (combo == null)
						{
							throw new KeyNotFoundException("Combo này không tồn tại");
						}
					}

					var orderDetail = new OrderDetail
					{
						DishId = dish?.DishId,
						ComboId = combo?.ComboId,
						Quantity = item.Quantity,
						UnitPrice = item.UnitPrice,
						Note = item.Note,
						OrderTime = item.OrderTime
					};

					orderDetails.Add(orderDetail);
				}

				if (orderDetails.Any())
				{
					var order = new Order
					{
						OrderDate = reservationDTO.OrderDate,
						Status = reservationDTO.Status ?? 0,
						RecevingOrder = reservationDTO.RecevingOrder,
						TotalAmount = reservationDTO.TotalAmount,
						OrderDetails = orderDetails,
						GuestPhone = guest.GuestPhone,
						AddressId = address.AddressId,
						Note = reservationDTO.Note,
						Deposits = reservationDTO.Deposits,
						Type = reservationDTO.Type
					};

					reservation.Order = order;
				}
			}

			_context.Reservations.Add(reservation);
			await _context.SaveChangesAsync();

			return reservation;
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
				r.Address.ConsigneeName.Contains(guestNameOrPhone));
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


		public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
		{
			return await _context.Reservations
				.AsNoTracking().Include(r => r.TableReservations)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
		}
		public async Task<List<Table>> GetAllTablesAsync()
		{
			return await _context.Tables
				.AsNoTracking()
				.ToListAsync();
		}



		public async Task<List<(Table, DateTime?)>> GetTablesWithCurrentDayReservationsAsync(int reservationId)
		{
			var reservation = await GetReservationByIdAsync(reservationId);

			if (reservation == null || !reservation.ReservationTime.HasValue)
			{
				return new List<(Table, DateTime?)>();
			}

			var reservationDate = reservation.ReservationTime.Value.Date;

			var tables = await _context.TableReservations
				.Where(tr =>
					tr.Reservation.Status == 1 &&
					tr.Reservation.ReservationTime.HasValue &&
					tr.Reservation.ReservationTime.Value.Date == reservationDate
				)
				.Select(tr => new { tr.Table, tr.Reservation.ReservationTime })
				.AsNoTracking()
				.ToListAsync();

			return tables.Select(x => (x.Table, x.ReservationTime)).ToList();
		}




		public async Task<List<(Table, DateTime?)>> GetTablesByReservationIdAsync(int reservationId)
		{
			return await _context.TableReservations
				.Where(tr => tr.ReservationId == reservationId)
				.Select(tr => new { tr.Table, tr.Reservation.ReservationTime })
				.AsNoTracking()
				.ToListAsync()
				.ContinueWith(task => task.Result.Select(x => (x.Table, x.ReservationTime)).ToList());
		}
       
    }
}
