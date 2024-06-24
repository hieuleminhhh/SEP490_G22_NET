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

			var address = await _repository.GetOrCreateAddress(reservationDTO.GuestPhone, reservationDTO.GuestAddress, reservationDTO.ConsigneeName);

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
				decimal totalAmount = 0;

				foreach (var item in reservationDTO.OrderDetails)
				{
					if (!item.DishId.HasValue || item.DishId == 0)
					{
						if (!item.ComboId.HasValue || item.ComboId == 0)
						{
							continue;
						}
					}

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

				if (orderDetails.Any())
				{
					var order = new Order
					{
						OrderDate = reservationDTO.OrderDate ?? DateTime.UtcNow,
						Status = reservationDTO.Status ?? 0, 
						TotalAmount = totalAmount,
						OrderDetails = orderDetails,
						GuestPhone = guest.GuestPhone,
						AddressId = address.AddressId,
						Note = reservationDTO.Note,
						Deposits = reservationDTO.Deposits
					};

					reservation.Order = order;
				}
			}

			await _repository.CreateReservationAsync(reservation);
		}

		public async Task<int> CountOrdersWithStatusOnDateAsync(DateTime date, int status)
		{
			return await _repository.CountOrdersWithStatusOnDateAsync(date, status);
		}

		public async Task<int> GetTotalTablesAsync()
		{
			return await _repository.GetTotalTablesAsync();
		}

		public async Task<IEnumerable<ReservationByStatus>> GetReservationsByStatus(int? status)
		{
			var reservations = await _repository.GetReservationsByStatus(status);
			var result = new List<ReservationByStatus>();

			foreach (var reservation in reservations)
			{
				var mappedReservation = _mapper.Map<ReservationByStatus>(reservation);
				mappedReservation.StatusOfTable = await CalculateStatusOfTable(reservation);
				result.Add(mappedReservation);
			}

			return result;
		}

		public async Task<int?> CalculateStatusOfTable(Reservation reservation)
		{
			if (reservation.ReservationTime == null)
			{
				return null;
			}

			var date = reservation.ReservationTime.Value.Date;

			var orderCountWithStatus1 = await _repository.CountOrdersWithStatusOnDateAsync(date, 1);
			var totalTables = await _repository.GetTotalTablesAsync();

			return orderCountWithStatus1 >= totalTables ? 1 : 0;
		}
	}
}
