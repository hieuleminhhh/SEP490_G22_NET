using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EHM_API.DTOs.TableDTO;
using EHM_API.Services;
using EHM_API.DTOs.TableDTO.Manager;
using Microsoft.AspNetCore.Authorization;

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
       
        [HttpGet("{id}")]
        public async Task <IActionResult> GetById(int id)
        {
            var table = await _service.GetTableByIdAsync(id);
            if (table == null)
                return NotFound();

            return Ok(table);
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
				Console.Error.WriteLine($"Lỗi: {ex}");
				return StatusCode(500, new { message = "Đã xảy ra lỗi khi tìm bàn. Vui lòng thử lại sau." });
			}
		}

		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<TableAllDTO>>> GetAllTables()
		{
			var tables = await _service.GetAllTablesAsync();
			return Ok(tables);
		}


        [HttpPut("{id}")]
        public async Task <IActionResult> UpdateAsync(int id, CreateTableDTO tabledto)
        {
			var tableid = await _service.GetTableByIdAsync(id);
		    var updatetable = await _service.UpdateAsync(id, tabledto);
            return Ok(updatetable);
        }


		[HttpPost]
		public async Task<IActionResult> Create(CreateTableDTO tabledto)
		{
			var table = await _service.AddAsync(tabledto);
			return Ok(table);
		}

        [HttpDelete("{tableId}")]
        public async Task<IActionResult> DeleteTable(int tableId)
        {
            try
            {
                await _service.DeleteTableWithDependenciesAsync(tableId);
                return Ok(new { message = "Table and related records deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-floor-to-null/{floor}")]
        public async Task<IActionResult> UpdateFloorToNull(int floor)
        {
            try
            {
                await _service.SetTablesFloorToNullAsync(floor);
                return Ok(new { message = "Floor set to null for all tables on the given floor." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
