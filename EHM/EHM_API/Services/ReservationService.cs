using AutoMapper;
using EHM_API.DTOs.OrderDTO.Guest;
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
        private readonly ITableRepository _tableRepository;
        private readonly ITableReservationRepository _tableReservationRepository;
        private readonly IMapper _mapper;

        public ReservationService(IReservationRepository repository,
		
			ITableRepository tableRepository,
			ITableReservationRepository tableReservationRepository,
			IMapper mapper)
        {
            _repository = repository;
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
			await _repository.CreateReservationAsync(reservationDTO);
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
        public async Task<ReasonCancelDTO?> UpdateReasonCancelAsync(int reservationId, ReasonCancelDTO? reasonCancelDTO)
        {
            if (reasonCancelDTO == null)
            {
                throw new ArgumentNullException(nameof(reasonCancelDTO));
            }

            var updatedReservation = await _repository.UpdateReasonCancelAsync(reservationId, reasonCancelDTO.ReasonCancel);

            if (updatedReservation == null)
            {
                return null;
            }

            return new ReasonCancelDTO
            {
                ReasonCancel = updatedReservation.ReasonCancel
            };
        }

		public async Task<GetReservationByOrderDTO?> GetReservationByOrderIdAsync(int orderId)
		{
			var reservation = await _repository.GetReservationByOrderIdAsync(orderId);

			if (reservation == null)
			{
				return null;
			}

			var dto = _mapper.Map<GetReservationByOrderDTO>(reservation);
			return dto;
		}
	}
}
