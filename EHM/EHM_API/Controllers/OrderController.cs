using AutoMapper;
using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.ComboDTO.Manager;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDTO.Guest;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.DTOs.TableDTO;
using EHM_API.DTOs.TableDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
		private readonly ITableService _tableService;
		private readonly IComboService _comboService;
		private readonly IDishService _dishService;

		private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper, ILogger<OrderController> logger, ITableService tableService, IComboService comboService, IDishService dishService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
            _dishService = dishService;
            _tableService = tableService;
            _comboService = comboService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDTO createOrderDTO)
        {


            var order = await _orderService.CreateOrderAsync(createOrderDTO);
            if (order == null)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn." });
            }

            var orderDTO = _mapper.Map<OrderDTOAll>(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = orderDTO.OrderId }, orderDTO);
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
                return NotFound(new { message = "Đơn đặt hàng không được tìm thấy." });
            }

            var updatedOrder = await _orderService.UpdateOrderAsync(id, updateOrderDTO);
            if (updatedOrder == null)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn." });
            }

            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var isCancelled = await _orderService.CancelOrderAsync(id);
                if (!isCancelled)
                {
                    return NotFound(new { message = "Đơn đặt hàng không được tìm thấy." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn." });
            }
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
                return StatusCode(500, "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.");
            }

            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            if (orders == null)
            {
                return StatusCode(500, "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.");
            }

            var orderDTOs = _mapper.Map<IEnumerable<OrderDTOAll>>(orders);
            return Ok(orderDTOs);
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchOrdersByGuestPhone(string guestPhone)
        {
            try
            {
                var orders = await _orderService.SearchOrdersAsync(guestPhone);
                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "Không tìm thấy đơn hàng nào theo số điện thoại của khách được cung cấp." });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn." });
            }
        }

        [HttpGet("GetListOrder")]
        public async Task<ActionResult<PagedResult<OrderDTO>>> GetListOrders(
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string? search = null,
     [FromQuery] string? dateFrom = null,
     [FromQuery] string? dateTo = null,
     [FromQuery] int status = 0,
     [FromQuery] string filterByDate = null,
     [FromQuery] int type = 0)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            DateTime? parsedDateFrom = null;
            DateTime? parsedDateTo = null;

            if (!string.IsNullOrEmpty(dateFrom))
            {
                if (DateTime.TryParseExact(dateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtFrom))
                {
                    parsedDateFrom = dtFrom;
                }
                else
                {
                    return BadRequest("Invalid date format for dateFrom. Please use yyyy-MM-dd.");
                }
            }

            if (!string.IsNullOrEmpty(dateTo))
            {
                if (DateTime.TryParseExact(dateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtTo))
                {
                    parsedDateTo = dtTo;
                }
                else
                {
                    return BadRequest("Invalid date format for dateTo. Please use yyyy-MM-dd.");
                }
            }

            var result = await _orderService.GetOrderAsync(
                search?.Trim(),
                parsedDateFrom,
                parsedDateTo,
                status,
                page,
                pageSize,
                filterByDate,
                type
            );

            return Ok(result);
        }



        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderDTO updateOrder)
        {
            if (updateOrder == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            var existingOrder = await _orderService.GetOrderByIdAsync(orderId);
            if (existingOrder == null)
            {
                return NotFound(new { message = "Không tìm thấy món ăn" });
            }

            var updatedOrder = await _orderService.UpdateOrderStatusAsync(orderId, updateOrder.Status);
            if (updatedOrder == null)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật trạng thái món ăn" });
            }

            return Ok(new
            {
                message = "Trạng thái đơn hàng được cập nhật thành công",
            });
        }



        [HttpGet("with-tables")]
        public async Task<ActionResult<IEnumerable<ListTableOrderDTO>>> GetOrdersWithTables()
        {
            var ordersWithTables = await _orderService.GetOrdersWithTablesAsync();
            return Ok(ordersWithTables);
        }

        [HttpGet("getOrderByTableId")]
        public async Task<IActionResult> GetOrderByTableId([FromQuery] int tableId)
        {
            var errors = new Dictionary<string, string>();

            if (tableId <= 0)
            {
                return BadRequest(new { message = "TableId không hợp lệ." });
            }

            try
            {
                var result = await _orderService.GetOrderByTableIdAsync(tableId);

                if (result == null)
                {
                    return BadRequest(new { message = "Không tìm thấy đơn hàng cho bàn này." });
                }

                return Ok(new
                {
                    message = "Lấy thông tin đơn hàng thành công.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý yêu cầu.", detail = ex.Message });
            }
        }

		[HttpPut("updateOrderDetails/{tableId}")]
		public async Task<IActionResult> UpdateOrderDetails(int tableId, [FromBody] UpdateTableAndGetOrderDTO dto)
		{
			if (tableId <= 0)
			{
				return BadRequest(new { message = "TableId không hợp lệ." });
			}

			var errors = new Dictionary<string, string>();

			if (dto.OrderId <= 0)
			{
				errors["OrderId"] = "OrderId không hợp lệ.";
				return BadRequest(errors);
			}

			var orderExists = await _orderService.GetOrderByIdAsync(dto.OrderId);
			if (orderExists == null)
			{
				return BadRequest(new { message = "Đơn hàng không tồn tại." });
			}

			foreach (var detail in dto.OrderDetails)
			{

				if (detail.UnitPrice <= 0)
				{
					errors["OrderDetails"] = "Giá của món ăn hoặc combo phải lớn hơn 0.";
				}

				if (detail.Quantity <= 0)
				{
					errors["OrderDetails"] = "Số lượng phải lớn hơn 0.";
				}
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			var result = await _orderService.UpdateOrderDetailsAsync(tableId, dto);

			if (result == null)
			{
				return NotFound(new { message = "Không tìm thấy đơn hàng hoặc bàn." });
			}

			var options = new JsonSerializerOptions
			{
				ReferenceHandler = ReferenceHandler.Preserve,
			};

			var jsonResult = JsonSerializer.Serialize(result, options);

			return Ok(new { message = "Cập nhật thành công.", data = jsonResult });
		}



		// Create Order for Table
		[HttpPost("createOrderForTable/{tableId}")]
		public async Task<IActionResult> CreateOrderForTable(int tableId, [FromBody] CreateOrderForTableDTO dto)
		{
			var errors = new Dictionary<string, string>();

			if (tableId <= 0)
			{
				errors["TableId"] = "Bàn không hợp lệ.";
				return BadRequest(errors);
			}

            var tableExists = await _tableService.ExistTable(tableId);
            if (!tableExists)
            {
                errors["TableId"] = "Bàn không tồn tại.";
                return BadRequest(errors);
            }
/*            if (dto.OrderDate == null || dto.OrderDate == DateTime.MinValue)
			{
				errors["Order Date"] = "Ngày đặt không hợp lệ.";
			}

			if (dto.RecevingOrder != null && dto.RecevingOrder <= DateTime.Now)
			{
				errors["Receving Date"] = "Ngày nhận không hợp lệ.";
			}*/

		/*	if (dto.TotalAmount <= 0)
			{
				errors["TotalAmount"] = "Tổng tiền không hợp lệ.";
			}


			if (dto.Type.HasValue && (dto.Type <= 1 || dto.Type >= 4))
			{
				errors["Type"] = "Loại đơn không hợp lệ.";
			}*/

			foreach (var detail in dto.OrderDetails)
			{
				if (!detail.DishId.HasValue && detail.ComboId <= 0)
				{
					errors["DishId"] = "Món ăn hoặc combo không được để trống.";
				}
				
				// Validate UnitPrice
				if (detail.UnitPrice <= 0)
				{
					errors["UnitPrice"] = "Đơn giá không hợp lệ.";
				}

				// Validate Quantity
				if (detail.Quantity <= 0)
				{
					errors["Quantity"] = "Số lượng phải lớn hơn 0.";
				}
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			try
			{
				var result = await _orderService.CreateOrderForTable(tableId, dto);
				if (result == null)
				{
					return BadRequest(new { message = "Không thể tạo đơn hàng." });
				}
				return Ok(new
				{
					message = "Đã tạo đơn hàng thành công."
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Đã xảy ra lỗi khi xử lý yêu cầu.");
				return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý yêu cầu.", detail = ex.ToString() });
			}
		}

	}
}
