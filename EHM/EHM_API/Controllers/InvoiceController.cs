﻿using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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


        [HttpPost("create-invoice/{orderId}")]
        public async Task<IActionResult> CreateInvoiceForOrderAsync(int orderId, [FromBody] CreateInvoiceForOrderDTO createInvoiceDto)
        {
            try
            {
                var invoiceId = await _invoiceService.CreateInvoiceForOrderAsync(orderId, createInvoiceDto);
                return Ok(new
                {
                    message = "Hóa đơn đã được tạo thành công và cập nhật vào đơn hàng.",
                    invoiceId = invoiceId // Include invoiceId in the response
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

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


	}
}
