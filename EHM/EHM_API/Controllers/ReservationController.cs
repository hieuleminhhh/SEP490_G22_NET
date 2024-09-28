using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.ReservationDTO.Manager;
using EHM_API.DTOs.Table_ReservationDTO;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class ReservationsController : ControllerBase
	{
		private readonly IReservationService _service;
		private readonly IDishService _dishService;
		private readonly IComboService _comboService;
		private readonly ITableService _tableService;
        private readonly EHMDBContext _context;

        public ReservationsController(IReservationService service, IDishService dishService, IComboService comboService, ITableService tableService, EHMDBContext context)
        {
            _service = service;
            _dishService = dishService;
            _comboService = comboService;
            _tableService = tableService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationByStatus>>> GetReservationsByStatus([FromQuery] int? status)
        {
            var reservations = await _service.GetReservationsByStatus(status);
            return Ok(reservations);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReservation(CreateReservationDTO createDto)
        {
            var errors = new Dictionary<string, string>();

            if (createDto == null)
            {
                return BadRequest(new { message = "Dữ liệu đặt bàn không hợp lệ." });
            }

            if (string.IsNullOrWhiteSpace(createDto.GuestPhone))
            {
                errors["guestPhone"] = "Số điện thoại khách hàng là bắt buộc.";
            }
            else if (!Regex.IsMatch(createDto.GuestPhone, @"^\d{10,15}$"))
            {
                errors["guestPhone"] = "Số điện thoại không hợp lệ.";
            }

            if (!string.IsNullOrWhiteSpace(createDto.Email) && !Regex.IsMatch(createDto.Email, @"^[\w\.-]+@[\w\.-]+\.\w+$"))
            {
                errors["email"] = "Email không hợp lệ.";
            }

            if (string.IsNullOrWhiteSpace(createDto.ConsigneeName))
            {
                errors["consigneeName"] = "Tên khách hàng là bắt buộc.";
            }

            if (createDto.ReservationTime == null)
            {
                errors["reservationTime"] = "Thời gian đặt bàn là bắt buộc.";
            }
            else if (createDto.ReservationTime <= DateTime.Now)
            {
                errors["reservationTime"] = "Thời gian đặt bàn không hợp lệ.";
            }

            if (createDto.GuestNumber == null)
            {
                errors["guestNumber"] = "Số lượng khách là bắt buộc.";
            }
            if (createDto.GuestNumber < 0)
            {
                errors["guestNumber"] = "Số lượng khách phải hợp lệ.";
            }

            if (createDto.TotalAmount == null || createDto.TotalAmount < 0)
            {
                errors["totalAmount"] = "Tổng tiền không hợp lệ.";
            }

            if (createDto.Deposits < 0)
            {
                errors["deposits"] = "Tiền đặt cọc không hợp lệ.";
            }

            if (createDto.OrderDetails != null)
            {
                foreach (var detail in createDto.OrderDetails)
                {
                    if (detail.Quantity <= 0)
                    {
                        errors["quantity"] = "Số lượng không hợp lệ.";
                    }
                    if (detail.UnitPrice == null || detail.UnitPrice < 0)
                    {
                        errors["unitPrice"] = "Giá tiền không hợp lệ.";
                    }

                    bool isDishIdValid = detail.DishId != null && detail.DishId > 0;
                    bool isComboIdValid = detail.ComboId != null && detail.ComboId > 0;

                    if (isDishIdValid && isComboIdValid)
                    {
                        errors["dishOrCombo"] = "Chỉ được chọn một trong hai: Món ăn hoặc Combo, không phải cả hai.";
                    }
                    else if (!isDishIdValid && !isComboIdValid)
                    {
                        errors["dishOrCombo"] = "Món ăn hoặc combo là bắt buộc.";
                    }
                    else
                    {
                        if (isDishIdValid && !await _dishService.DishExistsAsync(detail.DishId.Value))
                        {
                            errors["dishId"] = $"Món ăn với ID {detail.DishId} không tồn tại.";
                        }

                        if (isComboIdValid && !await _comboService.ComboExistsAsync(detail.ComboId.Value))
                        {
                            errors["comboId"] = $"Combo với ID {detail.ComboId} không tồn tại.";
                        }
                    }
                }
            }

            if (errors.Any())
            {
                return BadRequest(errors);
            }

            try
            {
                // Gọi service và trả về đối tượng DTO đã tạo
                var createdReservation = await _service.CreateReservationAsync(createDto);
                return Ok(new { message = "Đặt bàn thành công.", reservation = createdReservation });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo đặt bàn. Vui lòng thử lại sau." });
            }
        }


        [HttpGet("{id}")]
		public async Task<ActionResult<ReservationDetailDTO>> GetReservationDetail(int id)
		{
			try
			{
				if (id <= 0)
				{
					return BadRequest(new { message = "ID đặt bàn không hợp lệ." });
				}

				var reservationDetail = await _service.GetReservationDetailAsync(id);

				if (reservationDetail == null)
				{
					return NotFound(new { message = $"Không tìm thấy thông tin đặt bàn với ID {id}." });
				}

				return Ok(new
				{
					message = "Lấy thông tin đặt bàn thành công.",
					data = reservationDetail
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy thông tin đặt bàn. Vui lòng thử lại sau." });
			}
		}
		[Authorize(Roles = "OrderStaff, User")]
		[HttpPut("{reservationId}/update-status")]
        public async Task<IActionResult> UpdateStatus(int reservationId, [FromBody] UpdateStatusReservationDTO updateStatusReservationDTO)
        {
            try
            {
                await _service.UpdateStatusAsync(reservationId, updateStatusReservationDTO);
                return Ok(new { Message = "Trạng thái đặt bàn được cập nhật thành công." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Không tìm thấy đặt chỗ." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Không thể cập nhật Trạng thái. Lỗi: {ex.Message}" });
            }
        }

		[HttpGet("searchNameOrPhone")]
		public async Task<IActionResult> SearchReservations([FromQuery] string? guestNameOrguestPhone)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(guestNameOrguestPhone))
				{
					return BadRequest(new { message = "Tên khách hoặc số điện thoại không được bỏ trống." });
				}

				var results = await _service.SearchReservationsAsync(guestNameOrguestPhone);

				if (results == null || !results.Any())
				{
					return NotFound(new { message = "Không tìm thấy đặt chỗ nào theo thông tin đã cung cấp." });
				}

				return Ok(results);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn." });
			}
		}

		[Authorize(Roles = "OrderStaff")]
		[HttpPost("register-tables")]
        public async Task<IActionResult> RegisterTables([FromBody] RegisterTablesDTO registerTablesDTO)
        {
            try
            {
                await _service.RegisterTablesAsync(registerTablesDTO);
                return Ok(new { Message = "Bàn đã đăng ký thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Không thể đăng ký bàn. Lỗi: {ex.Message}" });
            }
        }





		[HttpGet("check-time/{reservationId}")]
		public async Task<IActionResult> GetReservationTime(int reservationId)
		{
			var result = await _service.GetReservationTimeAsync(reservationId);
			if (result == null)
			{
				return NotFound(new { message = "Không tìm thấy đặt bàn" });
			}
				
			return Ok(result);
		}


		[Authorize(Roles = "OrderStaff")]
		[HttpPut("{reservationId}/table/{tableId}/status")]
        public async Task<IActionResult> UpdateReservationAndTableStatus(int reservationId, int tableId, [FromBody] UpdateStatusReservationTable updateStatusReservationTable)
        {
            var errors = new Dictionary<string, string>();

            if (updateStatusReservationTable == null)
            {
                return BadRequest(new { message = "Dữ liệu cập nhật không hợp lệ." });
            }

            if (reservationId <= 0)
            {
                errors["reservationId"] = "ID đặt bàn không hợp lệ.";
            }

            if (tableId <= 0)
            {
                errors["tableId"] = "ID bàn không hợp lệ.";
            }

            if (errors.Any())
            {
                return BadRequest(errors);
            }

            try
            {
                await _service.UpdateReservationAndTableStatusAsync(reservationId, tableId, updateStatusReservationTable.ReservationStatus, updateStatusReservationTable.TableStatus);
                return Ok(new { message = "Cập nhật trạng thái đặt bàn và bàn thành công." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Không tìm thấy đặt chỗ hoặc bàn." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Đã xảy ra lỗi khi cập nhật trạng thái. Lỗi: {ex.Message}" });
			}
		}

		[Authorize(Roles = "OrderStaff")]
		[HttpPut("{reservationId}/tables/status")]
		public async Task<IActionResult> UpdateTableStatuses(int reservationId, [FromBody] UpdateStatusTableByReservation dto)
		{
			var result = await _service.UpdateTableStatusesAsync(reservationId, dto.TableStatus);
			if (!result)
			{
				return NotFound($"Reservation with ID {reservationId} not found.");
			}

			return NoContent();
		}

		[Authorize(Roles = "OrderStaff, User")]
		[HttpPut("{reservationId}/reason-cancel")]
        public async Task<IActionResult> UpdateReasonCancel(int reservationId, [FromBody] ReasonCancelDTO? reasonCancelDTO)
        {
            if (reasonCancelDTO == null)
            {
                return BadRequest("ReservationDTO is required.");
            }

            var result = await _service.UpdateReasonCancelAsync(reservationId, reasonCancelDTO);

            if (result == null)
            {
                return NotFound("Reservation not found or could not be updated.");
            }

            return Ok(result);
        }


		[HttpGet("GetReservationByOrderId/{orderId}")]
		public async Task<IActionResult> GetReservationByOrderId(int orderId)
		{
			var reservation = await _service.GetReservationByOrderIdAsync(orderId);

			if (reservation == null)
			{
				return NotFound(new { Message = $"Không tìm thấy đặt bàn cho đơn hàng {orderId}" });
			}

			return Ok(reservation);
		}


		[HttpGet("GetReservationsByTableId/{tableId}")]
		public async Task<ActionResult> GetReservationsByTableId(int tableId)
		{
			var reservations = await _service.GetReservationsByTableIdAsync(tableId);

			if (reservations == null)
			{
				return NotFound(new { Message = "Không tìm thấy đặt chỗ nào của bản này" });
			}

			return Ok(reservations);
		}


        [HttpPut("update-order")]
        public async Task<IActionResult> UpdateReservationOrder([FromBody] UpdateReservationOrderDTO dto)
        {
            var result = await _service.UpdateReservationOrderAsync(dto);
            if (!result)
            {
                return NotFound("Reservation not found");
            }
            return Ok("OrderID updated successfully");
        }


        [HttpPut("update-reservation-status")]
        public async Task<IActionResult> UpdateReservationStatus([FromBody] UpdateReservationStatusByOrder updateReservationStatusDto)
        {
            var result = await _service.UpdateReservationStatusAsync(updateReservationStatusDto);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

		[HttpGet("checkAvailability")]
		public IActionResult CheckReservation(DateTime reservationTime, int guestNumber)
		{
			if (reservationTime < DateTime.Now)
			{
				return BadRequest(new { CanReserve = false, Message = "Thời gian đặt bàn không hợp lệ." });
			}

			if (guestNumber <= 0)
			{
				return BadRequest(new { CanReserve = false, Message = "Số lượng khách phải lớn hơn 0." });
			}

			var result = _service.CheckReservation(reservationTime, guestNumber);

			return Ok(new { CanReserve = result.CanReserve, Message = result.Message });
		}

        [HttpGet]
        [Route("byDate")]
        public ActionResult<IEnumerable<TableReservationAllDTO>> GetByReservationTime([FromQuery] DateTime reservationTime)
        {
            var result = _tableService.GetTableReservationsByDate(reservationTime);
            if (result == null || !result.Any())
            {
                return NotFound("No reservations found for the specified date.");
            }
            return Ok(result);
        }

        [HttpPut("UpdateReservationAcceptBy")]
        public async Task<IActionResult> UpdateReservationAcceptBy([FromBody] UpdateReservationAcceptByDTO dto)
        {
            var result = await _service.UpdateReservationAcceptByAsync(dto);

            if (!result)
            {
                return NotFound("Reservation not found.");
            }

            return Ok("AcceptBy updated successfully.");
        }
        [HttpGet("{reservationId}/guest-email")]
        public async Task<IActionResult> GetGuestEmailByReservation(int reservationId)
        {
            // Tìm Reservation dựa trên ReservationId
            var reservation = await _context.Reservations
                .Include(r => r.Address) // Include Address để lấy thông tin từ Address
                .ThenInclude(a => a.GuestPhoneNavigation) // Include GuestPhoneNavigation để lấy thông tin từ Guest
                .Include(r => r.Account) // Include Account để lấy thông tin từ Account
                .Include(r => r.Order) // Include Order để lấy thông tin từ Order
                .Include(r => r.TableReservations) // Include TableReservations để lấy thông tin từ TableReservation
                    .ThenInclude(tr => tr.Table) // Include Table để lấy thông tin từ Table
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin đặt chỗ với ID đã cung cấp." });
            }

            // Lấy thông tin từ Reservation và Address
            var guestPhone = reservation.Address.GuestPhone;

            // Lấy thông tin email từ Guest thông qua GuestPhone
            var guest = reservation.Address.GuestPhoneNavigation;

            // Lấy ConsigneeName từ Address
            var consigneeNameFromAddress = reservation.Address?.ConsigneeName;

            // Lấy thông tin từ Order
            var orderId = reservation.OrderId;
            var order = reservation.Order;

            // Lấy thông tin từ Setting
            var setting = await _context.Settings.FirstOrDefaultAsync(); // Lấy thông tin đầu tiên từ Setting

            // Lấy thông tin bảng từ TableReservations
            var tablesInfo = reservation.TableReservations.Select(tr => new
            {
                tableId = tr.Table.TableId,
                status = tr.Table.Status,
                capacity = tr.Table.Capacity,
                floor = tr.Table.Floor,
                label = tr.Table.Lable
            }).ToList();

            // Trả về kết quả bao gồm các trường mới
            return Ok(new
            {
                reservationId = reservation.ReservationId,
                reservationTime = reservation.ReservationTime,
                guestNumber = reservation.GuestNumber,
                orderId = reservation.OrderId,
                guestPhone = guestPhone,
                consigneeName = consigneeNameFromAddress,
                email = guest.Email,
                eateryName = setting.EateryName,
                settingPhone = setting.Phone,
                settingAddress = setting.Address,
                settingEmail = setting.Email,
                openTime = setting.OpenTime,
                closeTime = setting.CloseTime,
                qrcode = setting.Qrcode,
                logo = setting.Logo,
                linkContact = setting.LinkContact,
                tables = tablesInfo // Thêm thông tin bảng vào kết quả trả về
            });
        }


    }
}
