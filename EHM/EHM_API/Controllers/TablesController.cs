using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EHM_API.Models;
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
        public async Task<ActionResult<IEnumerable<Table>>> GetTables()
        {
            return Ok(await _service.GetAllAsync());
        }

      
        [HttpGet("{id}")]
        public async Task<ActionResult<Table>> GetTable(int id)
        {
            var table = await _service.GetByIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            return Ok(table);
        }

      
        [HttpPost]
        public async Task<ActionResult<Table>> PostTable(Table table)
        {
            var createdTable = await _service.CreateAsync(table);
            return CreatedAtAction(nameof(GetTable), new { id = createdTable.TableId }, createdTable);
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTable(int id, Table table)
        {
            if (id != table.TableId)
            {
                return BadRequest();
            }

            await _service.UpdateAsync(table);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeTableStatus(int id, [FromBody] string status)
        {
            var result = await _service.ChangeStatusAsync(id, status);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

      
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Table>>> SearchTables([FromQuery] string keyword)
        {
            return Ok(await _service.SearchAsync(keyword));
        }
    }
}
