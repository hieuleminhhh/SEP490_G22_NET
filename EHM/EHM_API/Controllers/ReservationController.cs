using EHM_API.DTOs.ReservationDTO.Guest;
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

			if (createDto.OrderDate < DateTime.Now)
			{
				return BadRequest(new { message = "Ngày đặt bàn không thể là ngày trong quá khứ." });
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

		[HttpPut("{id}/updateTableId")]
		public async Task<IActionResult> UpdateReservationTable(int id, UpdateTableIdDTO updateTableIdDto)
		{
			if (id != updateTableIdDto.ReservationId)
			{
				return BadRequest(new { Message = "Reservation ID mismatch." });
			}

			var result = await _service.UpdateTableIdAsync(updateTableIdDto);
			if (result)
			{
				return Ok(new { Message = "TableId updated successfully." });
			}
			else
			{
				return NotFound(new { Message = "Reservation not found or could not update TableId." });
			}
		}

	}
}
