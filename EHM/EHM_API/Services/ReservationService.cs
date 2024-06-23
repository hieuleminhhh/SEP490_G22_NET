using AutoMapper;
using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.Models;
using EHM_API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Services
{
	public class ReservationService : IReservationService
	{
		private readonly IReservationRepository _repository;
		private readonly IDishRepository _Dishrepository;
		private readonly IComboRepository _Comborepository;
		private readonly IMapper _mapper;

		public ReservationService(
			IReservationRepository repository,
			IMapper mapper,
			IDishRepository dishrepository,
			IComboRepository comborepository)
		{
			_repository = repository;
			_mapper = mapper;
			_Dishrepository = dishrepository;
			_Comborepository = comborepository;
		}

		public async Task<IEnumerable<ReservationRequestDTO>> GetReservationsByStatusAsync(int? status)
		{
			var reservations = await _repository.GetReservationsByStatusAsync(status);
			reservations = reservations.Where(r => r.Status != -1).ToList();

			return _mapper.Map<IEnumerable<ReservationRequestDTO>>(reservations);
		}

		public async Task<bool> UpdateStatusAsync(UpdateStatusReservationDTO updateStatusDto)
		{
			return await _repository.UpdateStatusAsync(updateStatusDto);
		}

		public async Task<ReservationDetailDTO> GetReservationDetailAsync(int reservationId)
		{
			var reservation = await _repository.GetReservationDetailAsync(reservationId);
			return _mapper.Map<ReservationDetailDTO>(reservation);
		}

		public async Task<bool> UpdateTableIdAsync(UpdateTableIdDTO updateTableIdDTO)
		{
			return await _repository.UpdateTableIdAsync(updateTableIdDTO);
		}

		public async Task CreateReservationAsync(CreateReservationDTO reservationDTO)
		{
			var guest = await _repository.GetOrCreateGuest(reservationDTO.GuestPhone, reservationDTO.Email);

			var address = await _repository.GetOrCreateAddress(reservationDTO.GuestPhone,null, reservationDTO.ConsigneeName);

			var reservation = new Reservation
			{
				GuestPhone = guest.GuestPhone,
				ReservationTime = reservationDTO.ReservationTime,
				GuestNumber = reservationDTO.GuestNumber,
				Note = reservationDTO.Note,
				Status = 0, 
				TableId = reservationDTO.TableId,
				GuestPhoneNavigation = guest
			};

			// Step 4: Create Order if there are OrderDetails
			if (reservationDTO.OrderDetails != null && reservationDTO.OrderDetails.Any())
			{
				var orderDetails = new List<OrderDetail>();
				decimal totalAmount = 0;

				foreach (var item in reservationDTO.OrderDetails)
				{
					Dish dish = null;
					Combo combo = null;

					if (item.DishId.HasValue && item.DishId > 0)
					{
						dish = await _Dishrepository.GetDishByIdAsync(item.DishId.Value);
						if (dish == null)
						{
							throw new KeyNotFoundException($"Dish with ID {item.DishId} not found.");
						}
					}
					else if (item.ComboId.HasValue && item.ComboId > 0)
					{
						combo = await _Comborepository.GetComboByIdAsync(item.ComboId.Value);
						if (combo == null)
						{
							throw new KeyNotFoundException($"Combo with ID {item.ComboId} not found.");
						}
					}
					else
					{
						throw new InvalidOperationException("Each OrderDetail must have either DishId or ComboId.");
					}

					var unitPrice = dish != null ? dish.Price : combo?.Price;
					totalAmount += (unitPrice ?? 0) * item.Quantity;

					var orderDetail = new OrderDetail
					{
						DishId = dish?.DishId,
						ComboId = combo?.ComboId,
						Quantity = item.Quantity,
						UnitPrice = unitPrice
					};

					orderDetails.Add(orderDetail);
				}

				var order = new Order
				{
					OrderDate = DateTime.UtcNow,
					Status = 0, 
					TotalAmount = totalAmount,
					OrderDetails = orderDetails,
					GuestPhone = guest.GuestPhone,
					AddressId = address.AddressId,
					Note = reservationDTO.Note,
					Deposits = reservationDTO.Deposits
				};

				reservation.Order = order;
			}

			await _repository.CreateReservationAsync(reservation);
		}

		public async Task<IEnumerable<ReservationByStatus>> GetReservationsByStatus(int? status)
		{
			var reservations = await _repository.GetReservationsByStatus(status);
			return _mapper.Map<IEnumerable<ReservationByStatus>>(reservations);
		}
	}
}
