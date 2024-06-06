using AutoMapper;
using EHM_API.DTOs.OrderDTO;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
          
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDTO createOrderDTO)
        {
          

            var order = await _orderService.CreateOrderAsync(createOrderDTO);
            if (order == null)
            {
                return StatusCode(500, new { message = "A problem happened while handling your request." });
            }

            var orderDTO = _mapper.Map<OrderDTOAll>(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = orderDTO.OrderID }, orderDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var orderDTO = _mapper.Map<OrderDTOAll>(order);
            return Ok(orderDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, UpdateOrderDTO updateOrderDTO)
        {
         

            var existingOrder = await _orderService.GetOrderByIdAsync(id);
            if (existingOrder == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            var updatedOrder = await _orderService.UpdateOrderAsync(id, updateOrderDTO);
            if (updatedOrder == null)
            {
                return StatusCode(500, new { message = "A problem happened while handling your request." });
            }

            return NoContent();
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var existingOrder = await _orderService.GetOrderByIdAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            if (orders == null)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            var orderDTOs = _mapper.Map<IEnumerable<OrderDTOAll>>(orders);
            return Ok(orderDTOs);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<OrderDTOAll>>> SearchOrdersAsync([FromQuery] SearchOrdersRequestDTO request)
        {
            try
            {
                var guestPhone = request?.GuestPhone;
                if (string.IsNullOrWhiteSpace(guestPhone))
                {
                    var allOrders = await _orderService.GetAllOrdersAsync();
                    return Ok(allOrders);
                }

                var orders = await _orderService.SearchOrdersAsync(guestPhone);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
