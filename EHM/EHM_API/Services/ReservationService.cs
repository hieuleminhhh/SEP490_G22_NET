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
		private readonly IEmailService _emailService;

        public ReservationService(IReservationRepository repository, ITableRepository tableRepository, ITableReservationRepository tableReservationRepository, IMapper mapper, IEmailService emailService)
        {
            _repository = repository;
            _tableRepository = tableRepository;
            _tableReservationRepository = tableReservationRepository;
            _mapper = mapper;
            _emailService = emailService;
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

            if (reservation.Status == 3)
            {
                reservation.TimeIn = DateTime.Now; 
            }

            await _repository.UpdateReservationAsync(reservation);
        }


        public async Task<ReservationDetailDTO> GetReservationDetailAsync(int reservationId)
		{
			var reservation = await _repository.GetReservationDetailAsync(reservationId);
			return _mapper.Map<ReservationDetailDTO>(reservation);
		}

        public async Task<CReservationDTO> CreateReservationAsync(CreateReservationDTO reservationDTO)
        {
            return await _repository.CreateReservationAsync(reservationDTO);
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

    // Lưu lại các thay đổi về Reservation và Order để cập nhật một lần
    var reservationsToUpdate = new List<Reservation>();
    var ordersToUpdate = new List<Order>();

    // Lưu lại các tác vụ email và thông báo để chạy song song
    var emailTasks = new List<Task>();
    var notificationTasks = new List<Task>();

    // Biến lưu trữ thời gian delay giữa các email
    var delay = TimeSpan.FromSeconds(1);

    foreach (var reservation in reservations)
    {
        bool isExpired = false;

        // Kiểm tra trạng thái và quyết định cách so sánh ngày
        if (reservation.Status == 1)
        {
            // Nếu trạng thái = 1, so sánh với DateTime.Now
            if (reservation.ReservationTime.HasValue && reservation.ReservationTime.Value < DateTime.Now)
            {
                isExpired = true;
            }
        }
        else if (reservation.Status == 2)
        {
            // Nếu trạng thái = 2, so sánh với ngày hiện tại
            if (reservation.ReservationTime.HasValue && reservation.ReservationTime.Value.Date < DateTime.Now.Date)
            {
                isExpired = true;
            }
        }

        if (isExpired)
        {
            // Cập nhật trạng thái thành "Đã hủy" (status = 5)
            reservation.Status = 5;
            reservation.ReasonCancel = "Quá ngày nhận bàn, Khách không đến nhận đơn";
            reservation.CancelBy = "Hệ thống";

            if (reservation.Order != null)
            {
                reservation.Order.Status = 5;
                reservation.Order.CancelBy = "Hệ thống";
                reservation.Order.CancelationReason = reservation.ReasonCancel;
                ordersToUpdate.Add(reservation.Order);
            }

            reservationsToUpdate.Add(reservation);

            // Gửi email thông báo cho khách hàng
            var email = reservation.Address.GuestPhoneNavigation?.Email;

            // Kiểm tra nếu email null hoặc không hợp lệ
            if (!string.IsNullOrEmpty(email) && IsValidEmail(email))
            {
                var emailSubject = "Thông báo từ Eating House";
                var emailBody = $@"
                <p>Kính gửi Quý Khách,</p>
                <p>Chúng tôi rất tiếc phải thông báo rằng đơn đặt bàn của Quý Khách với mã đặt bàn <strong>{reservation.ReservationId}</strong> tại Eating House đã bị hủy.</p>
                <p>Lý do: {reservation.ReasonCancel}</p>
                <p>Cảm ơn Quý Khách đã tin tưởng và lựa chọn Eating House!</p>
                <p>Trân trọng,</p>
                <p>Eating House</p>";

                // Thêm tác vụ gửi email vào danh sách
                emailTasks.Add(SendEmailWithDelayAsync(email, emailSubject, emailBody, delay));
            }

            // Tạo thông báo cho khách hàng (có thể lưu vào hệ thống thông báo)
            var notification = new Notification
            {
                Description = $"Đơn đặt bàn {reservation.ReservationId} đã bị hủy. Lý do: {reservation.ReasonCancel}",
                AccountId = reservation.AccountId,
                OrderId = reservation.Order?.OrderId,
                Time = DateTime.Now
            };

            // Thêm tác vụ tạo thông báo vào danh sách
            notificationTasks.Add(Task.Run(() => _repository.CreateNotification(notification)));
        }

        // Ánh xạ reservation sang ReservationByStatus và thêm vào kết quả
        var mappedReservation = _mapper.Map<ReservationByStatus>(reservation);
        result.Add(mappedReservation);
    }

    // Cập nhật tất cả thay đổi về Reservation và Order trong một lần
    if (reservationsToUpdate.Any())
    {
        _repository.UpdateReservations(reservationsToUpdate);
    }
    if (ordersToUpdate.Any())
    {
        _repository.UpdateOrders(ordersToUpdate);
    }

    // Lưu tất cả thay đổi vào cơ sở dữ liệu một lần duy nhất
    await _repository.SaveChangesAsync();

    // Chạy song song các tác vụ gửi email và tạo thông báo
    await Task.WhenAll(emailTasks);
    await Task.WhenAll(notificationTasks);

    return result;
}

// Phương thức gửi email với delay
private async Task SendEmailWithDelayAsync(string email, string subject, string body, TimeSpan delay)
{
    try
    {
        await _emailService.SendEmailAsync(email, subject, body);
        await Task.Delay(delay); // Delay giữa các email
    }
    catch (Exception ex)
    {
        // Xử lý lỗi gửi email
        Console.WriteLine($"Lỗi gửi email cho {email}: {ex.Message}");
    }
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

			var updatedReservation = await _repository.UpdateReasonCancelAsync(reservationId, reasonCancelDTO.ReasonCancel, reasonCancelDTO.CancelBy);

			if (updatedReservation == null)
			{
				return null;
			}

			return new ReasonCancelDTO
			{
				ReasonCancel = updatedReservation.ReasonCancel,
				CancelBy = updatedReservation.CancelBy
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

			// Kiểm tra nếu tất cả các bàn đều bận (status = 1) và thời gian đặt bàn chưa đủ 1 tiếng
			bool isAllTablesBusy = allTables.All(t => t.Status == 1);
			bool isReservationTooSoon = (reservationTime - DateTime.Now).TotalMinutes < 60;

			if (isAllTablesBusy && isReservationTooSoon)
			{
				result.Message = "Bạn phải đặt bàn ít nhất trước 1 tiếng từ thời điểm hiện tại.";
				result.CanReserve = false;
				return result;
			}

			// Lọc các bàn trống (status = 0) và không bị bảo trì (status != 2)
			var tablesToConsider = allTables
				.Where(t => t.Status == 0 && t.Status != 2) // Chỉ lấy bàn trống (status = 0) và loại bỏ bàn bảo trì (status = 2)
				.ToList();

			// Tính tổng số chỗ trống cho tất cả các bàn trống
			int totalAvailableCapacity = tablesToConsider
				.Sum(t => t.Capacity ?? 0);

			// Lọc các đơn đặt bàn có trạng thái = 2 trong ngày hiện tại
			var reservationsAtReservationTime = existingReservations
				.Where(r => r.ReservationTime.HasValue && r.ReservationTime.Value.Date == reservationTime.Date && r.Status == 2) // Chỉ lấy đơn đặt bàn có status = 2
				.ToList();

			// Tính tổng số chỗ đã bị đặt bởi các đơn đặt bàn có status = 2
			int totalReservedCapacity = reservationsAtReservationTime
				.Sum(r => r.GuestNumber ?? 0);

			// Số chỗ còn trống sau khi trừ đi số chỗ bị đặt
			int remainingCapacity = totalAvailableCapacity - totalReservedCapacity;

			if (remainingCapacity >= guestNumber)
			{
				result.Message = $"Có thể đặt bàn. Còn trống {remainingCapacity} chỗ.";
				result.CanReserve = true;
			}
			else
			{
				result.Message = $"Không đủ bàn để phục vụ cho {guestNumber} người. Tổng số chỗ trống: {remainingCapacity}.";
				result.CanReserve = false;
			}

			return result;
		}


		private ReservationCheckResult CheckReservationForOtherDays(DateTime reservationTime, int guestNumber, List<Table> allTables, List<Reservation> existingReservations)
		{
			var result = new ReservationCheckResult(); // Tạo một đối tượng kết quả

			if (!existingReservations.Any())
			{
				int totalAvailableCapacityOtherDays = allTables
					.Where(t => t.Status != 2) // Loại bỏ các bàn đang bảo trì
					.Sum(t => t.Capacity ?? 0);

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

			var availableTables = allTables
				.Where(t => !reservedTableIds.Contains(t.TableId) && t.Status != 2)
				.ToList();

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



		public async Task<bool> UpdateReservationAcceptByAsync(UpdateReservationAcceptByDTO dto)
        {
            return await _repository.UpdateReservationAcceptByAsync(dto);
        }
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }

}