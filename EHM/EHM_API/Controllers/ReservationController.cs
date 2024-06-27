using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.ReservationDTO.Manager;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class ReservationsController : ControllerBase
	{
		private readonly IReservationService _service;

		public ReservationsController(IReservationService service)
		{
			_service = service;
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
			if (createDto == null)
			{
				return BadRequest(new { message = "Dữ liệu đặt bàn không hợp lệ." });
			}

			if (string.IsNullOrWhiteSpace(createDto.GuestPhone))
			{
				return BadRequest(new { message = "Số điện thoại khách hàng là bắt buộc." });
			}
			if (createDto.GuestNumber <= 0)
			{
				return BadRequest(new { message = "Số lượng khách phải lớn hơn 0." });
			}

			try
			{
				await _service.CreateReservationAsync(createDto);

				return Ok(new { Message = "Đặt bàn thành công." });
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

        [HttpPut("{reservationId}/update-status")]
        public async Task<IActionResult> UpdateStatus(int reservationId, [FromBody] UpdateStatusReservationDTO updateStatusReservationDTO)
        {
            try
            {
                await _service.UpdateStatusAsync(reservationId, updateStatusReservationDTO);
                return Ok(new { Message = "Status updated successfully." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Reservation not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to update Status. Error: {ex.Message}" });
            }
        }

        [HttpPost("register-tables")]
        public async Task<IActionResult> RegisterTables([FromBody] RegisterTablesDTO registerTablesDTO)
        {
            try
            {
                await _service.RegisterTablesAsync(registerTablesDTO);
                return Ok(new { Message = "Tables registered successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Failed to register tables. Error: {ex.Message}" });
            }
        }
    }
}
