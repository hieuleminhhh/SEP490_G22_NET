using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EHM_API.Services;
using EHM_API.DTOs.ReservationDTO.Manager;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableReservationController : ControllerBase
    {
        private readonly ITableReservationService _tableReservationService;

        public TableReservationController(ITableReservationService tableReservationService)
        {
            _tableReservationService = tableReservationService;
        }

        [HttpDelete("{reservationId}")]
        public async Task<IActionResult> DeleteTableReservation(int reservationId)
        {
            var result = await _tableReservationService.DeleteTableReservationByReservationIdAsync(reservationId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

		[HttpPost("CreateOrderandTable")]
		public async Task<IActionResult> CreateOrderTable([FromBody] CreateOrderTableDTO dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			await _tableReservationService.CreateOrderTablesAsync(dto);
			return Ok(new { Message = "Đã tạo đơn hàng và bảng thành công!" });
		}

	}
}
