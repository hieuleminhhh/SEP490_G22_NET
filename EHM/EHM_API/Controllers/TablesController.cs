using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EHM_API.Models;
using EHM_API.Services;
using EHM_API.DTOs.TableDTO;

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

	}
}
