using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EHM_API.DTOs.TableDTO;
using EHM_API.Services;
using EHM_API.DTOs.TableDTO.Manager;
using Microsoft.AspNetCore.Authorization;
using EHM_API.Repositories;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TablesController : ControllerBase
	{
		private readonly ITableService _service;
        private readonly ITableRepository _tableRepository;

        public TablesController(ITableService service, ITableRepository tableRepository)
        {
            _service = service;
            _tableRepository = tableRepository;
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

	/*	[Authorize(Roles = "OrderStaff,Cashier,Manager")]*/
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
                return Ok(new { message = "Table processed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-floor")]
        public async Task<IActionResult> UpdateFloor([FromBody] UpdateFloorDTO updateFloorDTO)
        {
            try
            {
                await _service.SetTablesFloorAsync(updateFloorDTO);
                return Ok(new { message = "Cập nhật tầng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }




        [HttpGet("status2-floor-null")]
        public async Task<IActionResult> GetTablesWithStatus2AndFloorNull()
        {
            var tables = await _tableRepository.GetTablesWithStatus2AndFloorNullAsync();
            return Ok(tables);
        }
        [HttpGet("available-tables")]
        public async Task<IActionResult> GetAvailableTables(DateTime reservationTime)
        {
            try
            {
                var availableTables = await _tableRepository.GetAvailableTablesAsync(reservationTime);
                return Ok(availableTables);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
