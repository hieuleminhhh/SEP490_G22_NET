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

		public async Task<IEnumerable<Reservation>> GetReservationsByStatusAsync(int? status)
		{
			if (status.HasValue)
			{
				return await _context.Reservations
					.Include(r => r.Order)
					.ThenInclude(o => o.Address)
					.Where(r => r.Status == status)
					.ToListAsync();
			}
			else
			{
				return await _context.Reservations
					.Include(r => r.Order)
					.ThenInclude(o => o.Address)
					.ToListAsync();
			}
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

		public async Task CreateReservationAsync(CreateReservationDTO reservationDTO)
		{
			// Step 1: Get or create Guest
			var guest = await GetOrCreateGuest(reservationDTO.GuestPhone, reservationDTO.Email);

			// Step 2: Get or create Address
			var address = await GetOrCreateAddress(reservationDTO.GuestPhone, reservationDTO.GuestAddress, reservationDTO.ConsigneeName);

			// Step 3: Create Reservation
			var reservation = new Reservation
			{
				GuestPhone = guest.GuestPhone,
				ReservationTime = reservationDTO.ReservationTime,
				GuestNumber = reservationDTO.GuestNumber,
				Note = reservationDTO.Note,
				Status = 0, // Assuming default status for new reservations
				TableId = null, // You may need to set the table ID if applicable
				Order = null, // Initialize Order later if needed
				GuestPhoneNavigation = guest,
				OrderId = null // Initialize Order ID later if needed
			};

			// Step 4: Create Order if there are OrderDetails
			if (reservationDTO.OrderDetails != null && reservationDTO.OrderDetails.Any())
			{
				var orderDetails = new List<OrderDetail>();

				foreach (var item in reservationDTO.OrderDetails)
				{
					if (item.DishId.HasValue && item.DishId > 0)
					{
						var dish = await _context.Dishes.FindAsync(item.DishId);
						if (dish == null)
						{
							throw new KeyNotFoundException($"Dish with ID {item.DishId} not found.");
						}

						var orderDetail = new OrderDetail
						{
							DishId = dish.DishId,
							Quantity = item.Quantity,
							UnitPrice = dish.Price // Or calculate based on business logic
												   // You may need to populate other properties like ComboId if needed
						};

						orderDetails.Add(orderDetail);
					}
					else if (item.ComboId.HasValue && item.ComboId > 0)
					{
						var combo = await _context.Combos.FindAsync(item.ComboId);
						if (combo == null)
						{
							throw new KeyNotFoundException($"Combo with ID {item.ComboId} not found.");
						}

						var orderDetail = new OrderDetail
						{
							ComboId = combo.ComboId,
							Quantity = item.Quantity,
							UnitPrice = combo.Price // Or calculate based on business logic
													// You may need to populate other properties like DishId if needed
						};

						orderDetails.Add(orderDetail);
					}
					else
					{
						throw new InvalidOperationException("Each OrderDetail must have either DishId or ComboId.");
					}
				}

				var order = new Order
				{
					OrderDate = DateTime.UtcNow,
					Status = 0, // Assuming default status for new orders
					TotalAmount = orderDetails.Sum(od => od.UnitPrice * od.Quantity),
					OrderDetails = orderDetails,
					GuestPhone = guest.GuestPhone,
					AddressId = address.AddressId,
					Note = reservationDTO.Note
				};

				reservation.Order = order;
			}

			_context.Reservations.Add(reservation);
			await _context.SaveChangesAsync();
		}

		public async Task CreateReservationAsync(Reservation reservation)
		{
			_context.Reservations.Add(reservation);
			await _context.SaveChangesAsync();
		}
	}
}
