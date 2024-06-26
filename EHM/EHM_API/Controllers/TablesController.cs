using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EHM_API.DTOs.TableDTO;
using EHM_API.Services;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TablesController : ControllerBase
	{
		private readonly ITableService _service;

		public TablesController(ITableService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<TableAllDTO>>> GetAllTables()
		{
			var tables = await _service.GetAllTablesAsync();
			return Ok(tables);
		}

		[HttpGet("available")]
		public async Task<IActionResult> GetAvailableTables([FromQuery] int guestNumber)
		{
			try
			{
				if (guestNumber <= 0)
				{
					return BadRequest(new { message = "Số lượng khách phải lớn hơn 0." });
				}

				var tables = await _service.GetAvailableTablesForGuestsAsync(guestNumber);

				if (!tables.Any())
				{
					return NotFound(new { message = "Không tìm thấy bàn phù hợp cho số lượng khách này." });
				}

				return Ok(new
				{
					message = "Tìm thấy bàn phù hợp.",
					data = tables
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Đã xảy ra lỗi khi tìm bàn. Vui lòng thử lại sau." });
			}
		}
	}
}
