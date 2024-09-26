using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.InvoiceDTO;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InvoiceController : ControllerBase
	{
		private readonly IInvoiceService _invoiceService;
		public InvoiceController(IInvoiceService incoiceService)
		{
			_invoiceService = incoiceService;
		}

		[HttpGet("{invoiceId}")]
		public async Task<IActionResult> GetInvoiceDetail(int invoiceId)
		{
			var invoiceDetail = await _invoiceService.GetInvoiceDetailAsync(invoiceId);
			if (invoiceDetail == null)
			{
				return NotFound(new { message = $"Không tìm thấy hóa đơn {invoiceId} với đơn này." });
			}

			return Ok(invoiceDetail);
		}

		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPost("create-invoice/{orderId}")]
        public async Task<IActionResult> CreateInvoiceForOrderAsync(int orderId, [FromBody] CreateInvoiceForOrderDTO createInvoiceDto)
        {
            try
            {
                var invoiceId = await _invoiceService.CreateInvoiceForOrderAsync(orderId, createInvoiceDto);
                return Ok(new
                {
                    message = "Hóa đơn đã được tạo thành công và cập nhật vào đơn hàng.",
                    invoiceId = invoiceId
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPost("createInvoiceForOrder/{orderId}")]
		public async Task<IActionResult> CreateInvoiceForOrder(int orderId, [FromBody] CreateInvoiceForOrder2DTO createInvoiceDto)
		{
			try
			{
				var invoiceId = await _invoiceService.CreateInvoiceForOrder(orderId, createInvoiceDto);
				return Ok(new
				{
					message = "Hóa đơn đã được tạo thành công và cập nhật vào đơn hàng.",
					invoiceId = invoiceId
				});
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}



		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPut("updateInvoice/{invoiceId}")]
		public async Task<IActionResult> UpdateInvoiceAndCreateGuest(int invoiceId, [FromBody] UpdateInvoiceDTO dto)
		{
			try
			{
				if (dto == null)
				{
					return BadRequest("Dữ liệu không hợp lệ");
				}

				await _invoiceService.UpdateInvoiceAndCreateGuestAsync(invoiceId, dto);

				return Ok(new
				{
					message = "Hóa đơn đã được cập nhật thành công và cập nhật vào đơn hàng."
				});
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

		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPut("updateSuccessPayment/{orderId}")]
		public async Task<IActionResult> UpdateInvoiceAndOrder(int orderId, [FromBody] UpdateInvoiceSuccessPaymentDTO dto)
		{
			try
			{
				await _invoiceService.UpdateInvoiceAndOrderAsync(orderId, dto);
				return Ok("Cập nhật hóa đơn và đơn hàng thành công.");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPut("updateStatus/{orderId}")]
		public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateStatusOrderDTO dto)
		{
			try
			{
				await _invoiceService.UpdateOrderStatusAsync(orderId, dto);
				return Ok("Cập nhật trạng thái đơn hàng thành công.");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}


		/*[Authorize(Roles = "OrderStaff,Cashier")]*/
		[HttpGet("GetInvoiceByOrderId/{orderId}")]
		public async Task<ActionResult<InvoiceDetailDTO>> GetInvoiceByOrderId(int orderId)
		{
			try
			{
				var invoiceDetail = await _invoiceService.GetInvoiceByOrderIdAsync(orderId);
				return Ok(invoiceDetail);
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
		[HttpPut("UpdateDepositAndCreateInvoice/{orderId}")]
		public async Task<IActionResult> UpdateDepositAndCreateInvoice(int orderId, [FromBody] PrepaymentDTO dto)
		{
			try
			{
				var invoiceId = await _invoiceService.UpdateDepositAndCreateInvoiceAsync(orderId, dto);
				return Ok(new { Message = "Đã cập nhật khoản tiền gửi và tạo hóa đơn thành công.", InvoiceId = invoiceId });
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
		[HttpPut("UpdateOrderAndInvoice/{orderId}")]
		public async Task<IActionResult> UpdateOrderAndInvoice(int orderId, InvoiceOfSitting dto)
		{
			try
			{
				await _invoiceService.UpdateOrderAndInvoiceAsync(orderId, dto);
				return Ok(new { Message = "Trạng thái đơn hàng và hóa đơn được cập nhật thành công." });
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

		[HttpGet("GetAllInvoiceAndOrder")]
		public async Task<ActionResult<IEnumerable<GetInvoiceAndOrderInfo>>> GetAllInvoicesAndOrders()
		{
			var result = await _invoiceService.GetAllInvoicesAndOrdersAsync();
			return Ok(result);
		}

		[HttpGet("GetCancelOrder")]
		public async Task<ActionResult<IEnumerable<GetOrderCancelInfo>>> GetInvoicesAndOrdersByStatusAndDeposit()
		{
			int status = 5;
			decimal minDeposit = 0;
			var result = await _invoiceService.GetOrdersWithStatusAndDepositAsync(status, minDeposit);
			return Ok(result);
		}

		[HttpGet("OrderUnpaidForShip")]
		public async Task<IActionResult> GetOrdersUnpaidForShip()
		{
			var orders = await _invoiceService.GetOrdersUnpaidForShipAsync();
			return Ok(orders);
		}


		[HttpPut("updatePaymentStatus/{orderId}")]
		public async Task<IActionResult> UpdatePaymentStatus(int orderId, [FromBody] UpdatePaymentStatusDTO dto)
		{
			var result = await _invoiceService.UpdatePaymentStatusAsync(orderId, dto);
			if (result)
			{
				return Ok(new { message = "Payment status updated successfully" });
			}

			return BadRequest(new { message = "Failed to update payment status" });
		}
        [HttpPut("update-invoice")]
        public async Task<IActionResult> UpdateInvoice([FromBody] UpdateAmountInvoiceDTO updateInvoiceDto)
        {
            var result = await _invoiceService.UpdateInvoiceAsync(updateInvoiceDto);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
