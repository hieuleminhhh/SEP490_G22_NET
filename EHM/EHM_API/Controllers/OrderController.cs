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
using EHM_API.Models;
using EHM_API.DTOs.OrderDetailDTO.Manager;
using Microsoft.AspNetCore.Authorization;
using EHM_API.DTOs.OrderDTO.Cashier;
using Microsoft.EntityFrameworkCore;

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
		private readonly EHMDBContext _context;
		private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, ITableService tableService, IComboService comboService, IDishService dishService, EHMDBContext context, ILogger<OrderController> logger, IMapper mapper)
        {
            _orderService = orderService;
            _tableService = tableService;
            _comboService = comboService;
            _dishService = dishService;
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize(Roles = "OrderStaff,Cashier")]
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


		[Authorize(Roles = "OrderStaff,Cashier")]
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


		[Authorize(Roles = "OrderStaff,Cashier")]
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

		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPost("updateOrderDetails/{tableId}")]
		public async Task<IActionResult> UpdateOrderDetails(int tableId, [FromBody] UpdateTableAndGetOrderDTO dto)
		{
			if (tableId <= 0)
			{
				return BadRequest(new { message = "TableId không hợp lệ." });
			}

			var errors = new Dictionary<string, string>();

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

			return Ok(new { message = "Cập nhật thành công." });
		}


		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPost("updateOrderDetailsByOrderId/{orderId}")]
		public async Task<IActionResult> UpdateOrderDetailsByOrderId(int orderId, [FromBody] UpdateTableAndGetOrderDTO dto)
		{
			if (orderId <= 0)
			{
				return BadRequest(new { message = "OrderId không hợp lệ." });
			}

			var errors = new Dictionary<string, string>();

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			var result = await _orderService.UpdateOrderDetailsByOrderIdAsync(orderId, dto);

			if (result == null)
			{
				return NotFound(new { message = "Không tìm thấy đơn hàng." });
			}

			var options = new JsonSerializerOptions
			{
				ReferenceHandler = ReferenceHandler.Preserve,
			};

			return Ok(new { message = "Cập nhật thành công." });
		}


		[Authorize(Roles = "OrderStaff,Cashier")]
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



		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPost("createOrderForReservation/{tableId}")]
		public async Task<IActionResult> CreateOrderForReservaion(int tableId, [FromBody] CreateOrderForReservaionDTO dto)
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
				var result = await _orderService.CreateOrderForReservation(tableId, dto);
				if (result == null)
				{
					return BadRequest(new { message = "Không thể tạo đơn hàng." });
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Đã xảy ra lỗi khi xử lý yêu cầu.");
				return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý yêu cầu.", detail = ex.ToString() });
			}
		}


		//Update status
		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPut("update-order-status-for-table")]
		public async Task<IActionResult> UpdateOrderStatusForTable(int tableId, int orderId, UpdateOrderStatusForTableDTO dto)
		{
			try
			{
				await _orderService.UpdateOrderStatusForTableAsync(tableId, orderId, dto);
				return Ok();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPut("cancel-order")]
		[Authorize(Roles = "OrderStaff,Cashier")]
		public async Task<IActionResult> CancelOrder(int tableId, int orderId, [FromBody] CancelOrderDTO dto)
		{
			try
			{
				await _orderService.CancelOrderForTableAsync(tableId, orderId, dto);
				return Ok(new { message = "Order cancelled successfully." });
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}


		[HttpGet("GetOrderDetails/{orderId}")]
		public async Task<ActionResult<IEnumerable<GetOrderDetailDTO>>> GetOrderDetails(int orderId)
		{
			var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(orderId);
			if (orderDetails == null || !orderDetails.Any())
			{
				return NotFound($"Không tìm thấy chi tiết đơn hàng cho OrderID {orderId}.");
			}
			return Ok(orderDetails);
		}


		[HttpGet("GetDishOrderDetails/{orderId}")]
		public async Task<ActionResult<IEnumerable<GetDishOrderDetailDTO>>> GetDishOrderDetails(int orderId)
		{
			var orderDetails = await _orderService.GetOrderDetailsByOrderId(orderId);
			if (orderDetails == null || !orderDetails.Any())
			{
				return NotFound($"Không tìm thấy chi tiết đơn hàng cho OrderID {orderId}.");
			}
			return Ok(orderDetails);
		}

		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPut("CancelOrderForTable/{tableId}")]
		public async Task<IActionResult> UpdateStatus(int tableId, [FromBody] CancelOrderTableDTO dto)
		{
			try
			{
				await _orderService.UpdateOrderAndTablesStatusAsyncByTableId(tableId, dto);

				return Ok(new { Message = "Đơn của bàn đã được cập nhật thành công" });
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { Message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật đơn hàng.", Error = ex.Message });
			}
		}

		/*[Authorize(Roles = "OrderStaff,Cashier")]*/
		[HttpPut("UpdateStatusAndCreateInvoice/{orderId}")]
		public async Task<IActionResult> UpdateStatusAndCreateInvoice(int orderId, UpdateStatusAndCInvoiceD dto)
		{
			try
			{
				var invoiceId = await _orderService.UpdateStatusAndCreateInvoiceAsync(orderId, dto);
				return Ok(new
				{
					Message = "Trạng thái đơn hàng được cập nhật và tạo hóa đơn thành công.",
					InvoiceID = invoiceId
				});
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { Message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
			}
		}

		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPut("UpdateAmountReceiving/{orderId}")]
		public async Task<IActionResult> UpdateAmountReceiving(int orderId, UpdateAmountReceiving dto)
		{
			try
			{
				await _orderService.UpdateAmountReceivingAsync(orderId, dto);
				return Ok(new { Message = "Số tiền nhận đã được cập nhật thành công." });
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { Message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
			}
		}


		[HttpPut("CancelOrderReason/{orderId}")]

		public async Task<IActionResult> UpdateCancelationReason(int orderId, [FromBody] CancelationReasonDTO? cancelationReasonDTO)
        {
            if (cancelationReasonDTO == null)
            {
                return BadRequest("Hãy điền lý do hủy đơn");
            }

            var result = await _orderService.UpdateCancelationReasonAsync(orderId, cancelationReasonDTO);

            if (result == null)
            {
                return NotFound("Không tìm thấy đơn hàng hoặc không thể cập nhật.");
            }

            return Ok(result);
        }

		[Authorize(Roles = "OrderStaff")]
		[HttpPost("AcceptOrder/{orderId}")]
		public async Task<IActionResult> AcceptOrder(int orderId, [FromBody] AcceptOrderDTO acceptOrderDto)
		{
			try
			{
				await _orderService.AcceptOrderAsync(orderId, acceptOrderDto);
				return Ok(new { Message = "Đơn hàng được chấp nhận thành công." });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		[HttpPut("update-account/{orderId}")]
        public async Task<ActionResult<OrderAccountDTO?>> UpdateAccountId(int orderId, [FromBody] UpdateOrderAccountDTO updateOrderAccountDTO)
        {
            var updatedOrder = await _orderService.UpdateAccountIdAsync(orderId, updateOrderAccountDTO);
            if (updatedOrder == null)
            {
                return NotFound();
            }
            return Ok(updatedOrder);
        }


        [HttpGet("orders/status/{status}/staff/{staffId}")]
        public async Task<ActionResult<IEnumerable<OrderDetailForStaffType1>>> GetOrdersByStatusAndAccountId(int status, int staffId)
        {
            var orders = await _orderService.GetOrdersByStatusAndAccountIdAsync(status, staffId);
            if (orders == null || !orders.Any())
            {
                return NotFound();
            }
            return Ok(orders);
        }


        [HttpPut("{orderId}/Updatestatus")]
        public async Task<ActionResult<Order>> UpdateOrderStatus(int orderId, [FromBody] UpdateStatusOrderDTO dto)
        {
            var updatedOrder = await _orderService.UpdateOrderStatusAsync(orderId, dto.Status);
            if (updatedOrder == null)
            {
                return NotFound();
            }
            return Ok(updatedOrder);
        }
        [HttpGet("statistics")]
        public async Task<ActionResult<List<OrderStatisticsDTO>>> GetOrderStatistics(DateTime? startDate, DateTime? endDate, int? collectedById)
        {
            var statistics = await _orderService.GetOrderStatisticsAsync(startDate, endDate, collectedById);
            return Ok(statistics);
        }




        [HttpGet("TotalSale-by-category")]
        public async Task<IActionResult> GetRevenueByCategory(DateTime? startDate, DateTime? endDate)
        {
            var revenues = await _orderService.GetSalesByCategoryAsync(startDate, endDate);
            return Ok(revenues);
        }
        [HttpGet("order/export/cashier")]
        public async Task<IActionResult> ExportCashier(string exportorderIds)
        {
            var orderIds = exportorderIds.Split(',').Select(int.Parse).ToList();
            var orderDetailsList = await _orderService.GetOrderDetailsByIdsAsync(orderIds);
            if (orderDetailsList == null || !orderDetailsList.Any())
            {
                return NotFound();
            }
            return Ok(orderDetailsList);
        }
        [HttpPut("update-total-amount/{orderId}")]
        public async Task<IActionResult> UpdateTotalAmount(int orderId)
        {
            var result = await _orderService.UpdateTotalAmountAsync(orderId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpGet("{orderId}/email")]
        public async Task<IActionResult> GetOrderEmailByOrderId(int orderId)
        {
            try
            {
                var orderEmailDTO = await _orderService.GetEmailByOrderIdAsync(orderId);
                if (orderEmailDTO == null)
                {
                    return NotFound(new { message = "Order not found or no associated email." });
                }

                return Ok(orderEmailDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("CheckDishInOrderDetails/{dishId}")]
        public async Task<ActionResult<bool>> CheckDishInOrderDetails(int dishId)
        {
            var dishExistsInOrder = await _context.OrderDetails
                .AnyAsync(od => od.DishId == dishId);
            return Ok(dishExistsInOrder);
        }
        [HttpGet("CheckComboInOrderDetails/{comboId}")]
        public async Task<ActionResult<bool>> CheckComboInOrderDetails(int comboId)
        {
            var comboExistsInOrder = await _context.OrderDetails
                .AnyAsync(od => od.ComboId == comboId);
            return Ok(comboExistsInOrder);
        }
        [HttpGet("CheckCategoryInDish/{categoryId}")]
        public async Task<ActionResult<bool>> CheckCategoryInDish(int categoryId)
        {
            var comboExistsInOrder = await _context.Dishes
                .AnyAsync(od => od.CategoryId == categoryId);
            return Ok(comboExistsInOrder);
        }
        [HttpPut("update-staff")]
        public async Task<IActionResult> UpdateStaffByOrderId([FromBody] UpdateStaffDTO updateStaffDTO)
        {
            var result = await _orderService.UpdateStaffByOrderIdAsync(updateStaffDTO);
            if (!result)
            {
                return NotFound("Order not found");
            }
            return Ok("StaffId updated successfully");
        }
        [HttpGet("Get-OrderStatus-8")]
        public async Task<IActionResult> GetOrdersWithStatus8()
        {
            var orders = await _orderService.GetOrdersWithStatus8Async();

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return Ok(orders);
        }

        [HttpPut("UpdateAcceptBy")]
        public async Task<IActionResult> UpdateAcceptBy([FromBody] UpdateAcceptByDTO dto)
        {
            var result = await _orderService.UpdateAcceptByAsync(dto);

            if (!result)
            {
                return NotFound("Order not found.");
            }

            return Ok("AcceptBy updated successfully.");
        }

        [HttpGet("cashier-report")]
        public async Task<IActionResult> GetCashierReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? collectedById)
        {
            // Gọi service với startDate và endDate, nếu không có giá trị thì sẽ lấy toàn bộ
            var report = await _orderService.GetCashierReportAsync(startDate, endDate, collectedById);
            return Ok(report);
        }
        [HttpGet("checkAccountID")]
        public async Task<IActionResult> GetAccountIdByOrderId(int orderId)
        {
            // Tìm Order dựa trên OrderId
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng với ID đã cung cấp." });
            }

            // Lấy AccountId từ Order
            var accountId = order.AccountId;

            if (accountId == null)
            {
                return NotFound(new { message = "Đơn hàng không có tài khoản liên kết." });
            }

            return Ok(new { AccountId = accountId });
        }
        [HttpGet("api/orders/by-cashier")]
        public async Task<IActionResult> GetOrdersByCashierAsync(DateTime? startDate, DateTime? endDate, int? collectedById)
        {
            // Đặt giá trị endDate là ngày hiện tại nếu không được cung cấp
            endDate = endDate.HasValue && endDate.Value.Date <= DateTime.Today
                ? endDate.Value.Date
                : DateTime.Today;

            // Lấy danh sách tất cả các Cashier
            var cashiersQuery = _context.Accounts
                .Where(a => a.Role == "Cashier" && a.IsActive == true);

            // Nếu lọc theo collectedById
            if (collectedById.HasValue)
            {
                cashiersQuery = cashiersQuery.Where(a => a.AccountId == collectedById);
            }

            var cashiers = await cashiersQuery.ToListAsync();

            // Bắt đầu truy vấn cho Orders
            var ordersQuery = _context.Orders
                .Where(o => o.Status == 4 &&  // Lọc các đơn hàng đã hoàn thành
                            o.Invoice.PaymentStatus == 1 &&  // Đơn hàng đã thanh toán
                            o.Invoice.PaymentTime.HasValue &&  // Có thời gian thanh toán
                            (!startDate.HasValue || o.Invoice.PaymentTime.Value.Date >= startDate.Value.Date) &&
                            o.Invoice.PaymentTime.Value.Date <= endDate)  // Trong khoảng thời gian yêu cầu
                .Include(o => o.Invoice)
                .Include(o => o.Collected); // Bao gồm thông tin tài khoản đã thu tiền

            var orders = await ordersQuery.ToListAsync();

            // Kết hợp Cashiers với Orders, đảm bảo rằng tất cả các Cashier đều được bao gồm
            var cashierOrderGroups = cashiers.GroupJoin(
                orders,
                cashier => cashier.AccountId,
                order => order.CollectedBy,
                (cashier, ordersGroup) => new { Cashier = cashier, Orders = ordersGroup }
            );

            // Tạo danh sách kết quả
            var result = new List<object>();

            // Duyệt qua từng Cashier và đơn hàng tương ứng
            foreach (var group in cashierOrderGroups)
            {
                var cashier = group.Cashier;
                var ordersForCashier = group.Orders;

                var ordersDto = ordersForCashier.Select(order => new
                {
                    order.OrderId,
                    order.OrderDate,
                    order.TotalAmount,
                    PaymentAmount = order.Invoice?.PaymentAmount,
                    PaymentStatus = order.Invoice?.PaymentStatus,
                    PaymentTime = order.Invoice?.PaymentTime
                }).ToList();

                result.Add(new
                {
                    CashierId = cashier.AccountId,
                    CashierName = $"{cashier.FirstName} {cashier.LastName}",
                    Orders = ordersDto
                });
            }

            return Ok(result);
        }
        [HttpGet("GetNoRefundOrderDetails")]
        public async Task<IActionResult> GetOrderSummary(DateTime? startDate = null, DateTime? endDate = null)
        {
            var now = DateTime.Now;

            // Lấy danh sách đơn hàng dựa trên các điều kiện
            var query = _context.Orders.AsQueryable();

            // Nếu có ngày bắt đầu và ngày kết thúc, áp dụng lọc
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(o =>
                    (o.RecevingOrder.HasValue && o.RecevingOrder >= startDate && o.RecevingOrder <= endDate) ||
                    (!o.RecevingOrder.HasValue && o.OrderDate >= startDate && o.OrderDate <= endDate));
            }

            // Thêm điều kiện cho từng loại đơn hàng
            var type2Orders = await query
                .Where(o => o.Status == 5 && o.Deposits > 0 && o.Type == 2)
                .ToListAsync();

            var type3Orders = await query
                .Where(o => o.Status == 5 && o.Deposits > 0 && o.Type == 3 &&
                            o.Reservations.Any(r => r.Status == 5 && r.ReservationTime < now))
                .ToListAsync();

            var type1Orders = await query
                .Where(o => o.Status == 5 && o.Deposits > 0 && o.Type == 1 &&
                            o.RecevingOrder < now)
                .ToListAsync();

            // Tính tổng số tiền đặt cọc
            var totalDeposits = type2Orders.Sum(o => o.Deposits) +
                                type3Orders.Sum(o => o.Deposits) +
                                type1Orders.Sum(o => o.Deposits);

            var result = new
            {
                TotalDeposits = totalDeposits,
                Type2Orders = type2Orders,
                Type3Orders = type3Orders,
                Type1Orders = type1Orders
            };

            return Ok(result);
        }
        [HttpGet("total-payment-amount")]
        public async Task<IActionResult> GetTotalPaymentAmount(DateTime? startDate = null, DateTime? endDate = null)
        {
            // Lấy tất cả các đơn hàng với các điều kiện đã chỉ định
            var orders = await _context.Orders
                .Include(o => o.Invoice)  // Include Invoice để có thông tin về PaymentAmount
                .Include(o => o.Invoice.Account) // Include Account của Invoice để kiểm tra role
                .Where(o => o.Status == 4 &&               // Order status = 4
                            o.Type == 2 &&                 // Type của Order = 2
                            o.Invoice.PaymentStatus == 1 && // Payment status = 1
                            o.Invoice.PaymentMethods == 1)  // Payment method = 1
                .Where(o => !startDate.HasValue || o.Invoice.PaymentTime >= startDate) // Kiểm tra startDate
                .Where(o => !endDate.HasValue || o.Invoice.PaymentTime <= endDate)     // Kiểm tra endDate
                .Where(o => o.Invoice.Account.Role != "Cashier") // Thêm điều kiện lọc role khác "cashier"
                .ToListAsync(); // Truy vấn danh sách đơn hàng

            // Biến để lưu tổng số tiền thanh toán
            decimal totalPaymentAmount = 0;

            // Danh sách invoiceId để hiển thị
            List<int> invoiceIds = new List<int>();

            // Duyệt qua tất cả các đơn hàng và tính tổng PaymentAmount
            foreach (var order in orders)
            {
                // Kiểm tra nếu PaymentAmount không null thì cộng vào tổng
                if (order.Invoice.PaymentAmount.HasValue)
                {
                    totalPaymentAmount += order.Invoice.PaymentAmount.Value;

                    // Thêm InvoiceId vào danh sách
                    invoiceIds.Add(order.Invoice.InvoiceId);
                }
            }

            // Trả về kết quả với tổng số tiền thanh toán và danh sách invoiceId
            return Ok(new
            {
                TotalPaymentAmount = totalPaymentAmount,
                InvoiceIds = invoiceIds // Hiển thị danh sách InvoiceId
            });
        }


    }
}
