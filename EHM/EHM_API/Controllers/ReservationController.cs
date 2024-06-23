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

/*		[HttpGet]
		public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations(int? status)
		{
			var reservations = await _service.GetReservationsByStatusAsync(status);
			return Ok(reservations);
		}
*/

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
				return BadRequest("Reservation data is null.");
			}

			if (string.IsNullOrWhiteSpace(createDto.GuestPhone))
			{
				return BadRequest("GuestPhone is required.");
			}

			try
			{
				await _service.CreateReservationAsync(createDto);

				return Ok(new { Message = "Reservation created successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Message = $"Failed to create reservation. Error: {ex.Message}" });
			}
		}



		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateReservationStatus(int id, UpdateStatusReservationDTO updateStatusDto)
		{
			if (id != updateStatusDto.ReservationId)
			{
				return BadRequest(new { Message = "Reservation ID mismatch." });
			}

			var result = await _service.UpdateStatusAsync(updateStatusDto);
			if (result)
			{
				return Ok(new { Message = "Reservation status updated successfully." });
			}
			else
			{
				return NotFound(new { Message = "Reservation not found or could not be updated." });
			}
		}

		// New API to get reservation detail
		[HttpGet("{id}")]
		public async Task<ActionResult<ReservationDetailDTO>> GetReservationDetail(int id)
		{
			var reservationDetail = await _service.GetReservationDetailAsync(id);
			if (reservationDetail == null)
			{
				return NotFound(new { Message = "Reservation not found." });
			}

			return Ok(reservationDetail);
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
