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

		public async Task<GetReservationByOrderDTO> GetReservationsByTableIdAsync(int tableId)
		{
			var reservations = await _repository.GetReservationsByTableIdAsync(tableId);
			return _mapper.Map<GetReservationByOrderDTO>(reservations);
		}
		public async Task<bool> UpdateReservationOrderAsync(UpdateReservationOrderDTO dto)
		{
			var reservation = await _repository.GetReservationByIdAsync(dto.ReservationId);
			if (reservation == null)
			{
				return false;
			}

			await _repository.UpdateReservationOrderAsync(dto.ReservationId, dto.OrderId);
			return true;
		}


		public async Task<UpdateReservationStatusByOrder?> UpdateReservationStatusAsync(UpdateReservationStatusByOrder updateReservationStatusByOrder)
		{
			var reservation = await _repository.GetReservationByOrderIdAsync(updateReservationStatusByOrder.OrderId);
			if (reservation == null)
			{
				return null;
			}

			reservation.Status = updateReservationStatusByOrder.Status;

			await _repository.UpdateReservationAsync(reservation);

			return _mapper.Map<UpdateReservationStatusByOrder>(reservation);
		}

		public ReservationCheckResult CheckReservation(DateTime reservationTime, int guestNumber)
		{
			if (reservationTime < DateTime.Now)
			{
				return new ReservationCheckResult
				{
					CanReserve = false,
					Message = "Thời gian đặt bàn không hợp lệ."
				};
			}
			if (guestNumber <= 0)
			{
				return new ReservationCheckResult
				{
					CanReserve = false,
					Message = "Số lượng khách phải lớn hơn 0."
				};
			}

			var allTables = _repository.GetAllTables().ToList();
			var totalRestaurantCapacity = allTables.Sum(t => t.Capacity ?? 0);

			var existingReservations = _repository.GetReservationsForDateTime(reservationTime).ToList();
			var totalReservedGuests = existingReservations.Sum(r => r.GuestNumber ?? 0);

			if (totalReservedGuests >= totalRestaurantCapacity)
			{
				var nextAvailableTime = reservationTime.AddHours(3);
				return new ReservationCheckResult
				{
					CanReserve = false,
					Message = $"Hết bàn trong giờ này. Bạn có thể đặt bàn vào lúc {nextAvailableTime:HH:mm} hoặc sau đó."
				};
			}

			if (reservationTime.Date == DateTime.Now.Date)
			{
				return CheckReservationForToday(reservationTime, guestNumber, allTables, existingReservations);
			}
			else
			{
				return CheckReservationForOtherDays(reservationTime, guestNumber, allTables, existingReservations);
			}
		}

		private ReservationCheckResult CheckReservationForToday(DateTime reservationTime, int guestNumber, List<Table> allTables, List<Reservation> existingReservations)
		{
			var result = new ReservationCheckResult(); // Tạo một đối tượng kết quả

			if (existingReservations == null || !existingReservations.Any())
			{
				int totalAvailableCapacity = allTables.Sum(t => t.Capacity ?? 0);

				if (totalAvailableCapacity >= guestNumber)
				{
					result.Message = $"Có thể đặt bàn. Còn trống {totalAvailableCapacity} chỗ.";
					result.CanReserve = true; // Giả sử bạn có thuộc tính IsSuccess trong ReservationCheckResult
				}
				else
				{
					result.Message = $"Không đủ bàn để phục vụ cho {guestNumber} người. Tổng số chỗ trống: {totalAvailableCapacity}.";
					result.CanReserve = false;
				}

				return result;
			}

			var reservedTableIds = existingReservations
				.Where(r => r.TableReservations != null)
				.SelectMany(r => r.TableReservations.Select(tr => tr.TableId))
				.Distinct()
				.ToList();

			var availableTables = allTables
				.Where(t => !reservedTableIds.Contains(t.TableId) &&
							(t.Status == 0 || reservationTime > DateTime.Now.AddHours(3)))
				.ToList();

			int totalAvailableCapacityToday = availableTables.Sum(t => t.Capacity ?? 0);
			int totalReservedCapacity = existingReservations.Sum(r => r.GuestNumber ?? 0);
			int remainingCapacityToday = totalAvailableCapacityToday - totalReservedCapacity;

			if (remainingCapacityToday < guestNumber)
			{
				result.Message = $"Không đủ bàn để phục vụ cho {guestNumber} người. Tổng số chỗ trống: {remainingCapacityToday}.";
				result.CanReserve = false;
				return result;
			}

			var suitableTable = availableTables.FirstOrDefault(t => t.Capacity >= guestNumber);

			if (suitableTable != null)
			{
				result.Message = $"Có thể đặt bàn. Còn trống {remainingCapacityToday} chỗ.";
				result.CanReserve = true;
			}
			else if (remainingCapacityToday >= guestNumber)
			{
				result.Message = $"Có thể đặt bàn bằng cách ghép bàn. Còn trống {remainingCapacityToday} chỗ.";
				result.CanReserve = true;
			}
			else
			{
				result.Message = $"Không đủ bàn để phục vụ cho {guestNumber} người. Tổng số chỗ trống: {remainingCapacityToday}.";
				result.CanReserve = false;
			}

			return result;
		}

		private ReservationCheckResult CheckReservationForOtherDays(DateTime reservationTime, int guestNumber, List<Table> allTables, List<Reservation> existingReservations)
		{
			var result = new ReservationCheckResult(); // Tạo một đối tượng kết quả

			if (existingReservations == null || !existingReservations.Any())
			{
				int totalAvailableCapacityOtherDays = allTables.Sum(t => t.Capacity ?? 0);

				if (totalAvailableCapacityOtherDays >= guestNumber)
				{
					result.Message = $"Tất cả các bàn đều trống tại thời điểm {reservationTime:HH:mm}. Còn trống {totalAvailableCapacityOtherDays} chỗ.";
					result.CanReserve = true;
				}
				else
				{
					result.Message = $"Không đủ bàn để phục vụ cho {guestNumber} người. Tổng số chỗ trống: {totalAvailableCapacityOtherDays}.";
					result.CanReserve = false;
				}

				return result;
			}

			var reservedTableIds = existingReservations
				.Where(r => r.TableReservations != null)
				.SelectMany(r => r.TableReservations.Select(tr => tr.TableId))
				.Distinct()
				.ToList();

			var availableTables = allTables.Where(t => !reservedTableIds.Contains(t.TableId)).ToList();

			if (!availableTables.Any())
			{
				result.Message = $"Hết bàn trong giờ này. Không đủ bàn để phục vụ cho {guestNumber} người.";
				result.CanReserve = false;
				return result;
			}

			int totalAvailableCapacity = availableTables.Sum(t => t.Capacity ?? 0);
			int totalReservedCapacity = existingReservations.Sum(r => r.GuestNumber ?? 0);
			int remainingCapacity = totalAvailableCapacity - totalReservedCapacity;

			if (remainingCapacity < guestNumber)
			{
				result.Message = $"Không đủ bàn để phục vụ cho {guestNumber} người. Tổng số chỗ trống: {remainingCapacity}.";
				result.CanReserve = false;
				return result;
			}

			var suitableTable = availableTables.FirstOrDefault(t => t.Capacity >= guestNumber);

			if (suitableTable != null)
			{
				result.Message = $"Có thể đặt bàn. Còn trống {remainingCapacity} chỗ.";
				result.CanReserve = true;
			}
			else if (remainingCapacity >= guestNumber)
			{
				result.Message = $"Có thể đặt bàn bằng cách ghép bàn. Còn trống {remainingCapacity} chỗ.";
				result.CanReserve = true;
			}
			else
			{
				result.Message = $"Không đủ bàn để phục vụ cho {guestNumber} người. Tổng số chỗ trống: {remainingCapacity}.";
				result.CanReserve = false;
			}

			return result;
		}

	}
}