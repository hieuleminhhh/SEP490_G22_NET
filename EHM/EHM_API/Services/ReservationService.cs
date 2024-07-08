using AutoMapper;
using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.ReservationDTO.Manager;
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
        private readonly ITableRepository _tableRepository;
        private readonly ITableReservationRepository _tableReservationRepository;
        private readonly IMapper _mapper;

        public ReservationService(IReservationRepository repository,
			IDishRepository dishrepository,
			IComboRepository comborepository,
			ITableRepository tableRepository,
			ITableReservationRepository tableReservationRepository,
			IMapper mapper)
        {
            _repository = repository;
            _Dishrepository = dishrepository;
            _Comborepository = comborepository;
            _tableRepository = tableRepository;
            _tableReservationRepository = tableReservationRepository;
            _mapper = mapper;
        }

        public async Task UpdateStatusAsync(int reservationId, UpdateStatusReservationDTO updateStatusReservationDTO)
        {
            var reservation = await _repository.GetReservationDetailAsync(reservationId);
            if (reservation == null)
            {
                throw new KeyNotFoundException("Không tìm thấy đặt bàn này");
            }

            reservation.Status = updateStatusReservationDTO.Status;

            if (reservation.Order != null && updateStatusReservationDTO.Status == 2)
            {
                reservation.Order.Status = 2;
            }

            await _repository.UpdateReservationAsync(reservation);
        }

        public async Task<ReservationDetailDTO> GetReservationDetailAsync(int reservationId)
		{
			var reservation = await _repository.GetReservationDetailAsync(reservationId);
			return _mapper.Map<ReservationDetailDTO>(reservation);
		}

		public async Task CreateReservationAsync(CreateReservationDTO reservationDTO)
		{
			var guest = await _repository.GetOrCreateGuest(reservationDTO.GuestPhone, reservationDTO.Email);

			var address = await _repository.GetOrCreateAddress(
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
						UnitPrice = item.UnitPrice
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
        public async Task RegisterTablesAsync(RegisterTablesDTO registerTablesDTO)
        {
            var reservation = await _repository.GetReservationDetailAsync(registerTablesDTO.ReservationId);

            if (reservation == null)
            {
                throw new KeyNotFoundException("Không tìm thấy đặt bàn này.");
            }

            foreach (var tableId in registerTablesDTO.TableIds)
            {
                var table = await _tableRepository.GetTableByIdAsync(tableId);

                if (table == null)
                {
                    throw new KeyNotFoundException($"Bàn không tồn tại");
                }

                var tableReservation = new TableReservation
                {
                    ReservationId = reservation.ReservationId,
                    TableId = table.TableId
                };

                await _tableReservationRepository.AddTableReservationAsync(tableReservation);
            }

            await _repository.UpdateReservationAsync(reservation);
        }


		public async Task<IEnumerable<ReservationSearchDTO>> SearchReservationsAsync(string? guestNameOrPhone)
		{
			var reservations = await _repository.SearchReservationsAsync(guestNameOrPhone);
			return _mapper.Map<IEnumerable<ReservationSearchDTO>>(reservations);
		}

		public async Task<CheckTimeReservationDTO?> GetReservationTimeAsync(int reservationId)
		{
			var reservation = await _repository.GetReservationByIdAsync(reservationId);

			if (reservation == null)
			{
				return null;
			}

			List<(Table, DateTime?)> tablesWithCurrentDayReservations = await _repository.GetTablesWithCurrentDayReservationsAsync(reservationId);
			List<Table> allTables = await _repository.GetAllTablesAsync();
			List<(Table, DateTime?)> tablesByReservation = await _repository.GetTablesByReservationIdAsync(reservationId);

			var todayTables = tablesWithCurrentDayReservations.Select(t => new CheckTimeTableDTO
			{
				TableId = t.Item1.TableId,
				Status = t.Item1.Status,
				Capacity = t.Item1.Capacity,
				Floor = t.Item1.Floor,
				ReservationTime = t.Item2
			}).ToList();

			return new CheckTimeReservationDTO
			{
				ReservationTime = reservation.ReservationTime,
				CurrentDayReservationTables = todayTables,
				AllTables = allTables.Select(t => new CheckTimeTableDTO
				{
					TableId = t.TableId,
					Status = t.Status,
					Capacity = t.Capacity,
					Floor = t.Floor
				}).ToList()
			};
		}
        public async Task UpdateReservationAndTableStatusAsync(int reservationId, int tableId, int reservationStatus, int tableStatus)
        {
            var reservation = await _repository.GetReservationDetailAsync(reservationId);
            if (reservation == null)
            {
                throw new KeyNotFoundException("Không tìm thấy đặt bàn này");
            }

            var table = await _tableRepository.GetTableByIdAsync(tableId);
            if (table == null)
            {
                throw new KeyNotFoundException("Bàn không tồn tại");
            }

            reservation.Status = reservationStatus;
            table.Status = tableStatus;

            await _repository.UpdateReservationAsync(reservation);
            await _tableRepository.UpdateTableAsync(table);
        }
        public async Task<bool> UpdateTableStatusesAsync(int reservationId, int newStatus)
        {
            var reservation = await _repository.GetReservationByIdAsync(reservationId);
            if (reservation == null)
            {
                return false;
            }

            var tableIds = reservation.TableReservations.Select(tr => tr.TableId).ToList();
            var tables = await _tableRepository.GetListTablesByIdsAsync(tableIds);

            foreach (var table in tables)
            {
                table.Status = newStatus;
            }

            await _tableRepository.UpdateListTablesAsync(tables);

            return true;
        }
    }
}
