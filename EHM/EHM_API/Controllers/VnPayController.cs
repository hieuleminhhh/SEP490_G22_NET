using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.VnPayDTO;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IVnPayService _vnPayservice;

        public VnPayController(IVnPayService vnPayservice)
        {
            _vnPayservice = vnPayservice;
        }

        [HttpPost("checkout-order")]
        public IActionResult GetVnPay([FromBody] CheckoutSuccessDTO checkoutDTO)
        {
            try
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = checkoutDTO.TotalAmount,
                    CreatedDate = checkoutDTO.OrderDate,
                    Description = checkoutDTO.Note,
                    FullName = checkoutDTO.ConsigneeName,
                    OrderId = checkoutDTO.OrderId
                };

                var vnPayUrl = _vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel);

                return Ok(new { url = vnPayUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpPost("checkout-reservation")]
        public IActionResult GetVnPays([FromBody] CreateReservationDTO checkoutDTO)
        {
            try
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = checkoutDTO.TotalAmount,
                    CreatedDate = checkoutDTO.OrderDate,
                    Description = checkoutDTO.Note,
                    FullName = checkoutDTO.ConsigneeName,
                    OrderId = 123
                };

                var vnPayUrl = _vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel);

                return Ok(new { url = vnPayUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }
    }


}
