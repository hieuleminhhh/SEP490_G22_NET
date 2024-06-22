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
			// Step 1: Get or create Guest
			var guest = await _repository.GetOrCreateGuest(reservationDTO.GuestPhone, reservationDTO.Email);

			// Step 2: Get or create Address
			var address = await _repository.GetOrCreateAddress(reservationDTO.GuestPhone, reservationDTO.GuestAddress, reservationDTO.ConsigneeName);

			// Step 3: Create Reservation
			var reservation = new Reservation
			{
				GuestPhone = guest.GuestPhone,
				ReservationTime = reservationDTO.ReservationTime,
				GuestNumber = reservationDTO.GuestNumber,
				Note = reservationDTO.Note,
				Status = 0, // Assuming default status for new reservations
				TableId = reservationDTO.TableId,
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
						var dish = await _Dishrepository.GetDishByIdAsync(item.DishId.Value);
						if (dish == null)
						{
							throw new KeyNotFoundException($"Dish with ID {item.DishId} not found.");
						}

						var orderDetail = new OrderDetail
						{
							DishId = dish.DishId,
							Quantity = item.Quantity,
							UnitPrice = dish.Price // Or calculate based on business logic
						};

						orderDetails.Add(orderDetail);
					}
					else if (item.ComboId.HasValue && item.ComboId > 0)
					{
						var combo = await _Comborepository.GetComboByIdAsync(item.ComboId.Value);
						if (combo == null)
						{
							throw new KeyNotFoundException($"Combo with ID {item.ComboId} not found.");
						}

						var orderDetail = new OrderDetail
						{
							ComboId = combo.ComboId,
							Quantity = item.Quantity,
							UnitPrice = combo.Price // Or calculate based on business logic
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

			await _repository.CreateReservationAsync(reservation);
		}
	}
}
