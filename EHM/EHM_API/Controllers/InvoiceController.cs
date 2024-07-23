using EHM_API.DTOs.CartDTO.OrderStaff;
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
        public async Task<IActionResult> CreateInvoiceForOrder(int orderId, [FromBody] CreateInvoiceForOrderDTO createInvoiceDto)
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

    }
}
