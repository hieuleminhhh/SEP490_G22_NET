using EHM_API.DTOs.OrderTableDTO;
using EHM_API.DTOs.TableDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTableController : ControllerBase
    {
        private readonly IOrderTableService _orderTableService;

        public OrderTableController(IOrderTableService orderTableService)
        {
            _orderTableService = orderTableService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderTableAsync(CreateOrderTable createOrderTableDTO)
        {
            var ordertable = await _orderTableService.CreateOrderTableAsync(createOrderTableDTO);
            return Ok(ordertable);
        }
    }
}
