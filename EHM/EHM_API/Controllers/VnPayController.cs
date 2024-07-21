using EHM_API.DTOs.CartDTO.Guest;
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

        [HttpPost]
        public IActionResult GetVnPay([FromBody] CheckoutDTO checkoutDTO)
        {
            try
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = checkoutDTO.TotalAmount,
                    CreatedDate = checkoutDTO.OrderDate,
                    Description = checkoutDTO.Note,
                    FullName = checkoutDTO.ConsigneeName,
                    OrderId = new Random().Next(1000, 100000)
                };

                var vnPayUrl = _vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel);

                return Ok(new { url = vnPayUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /*[HttpGet("PaymentCallBack")]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            // Tạo URL điều hướng với kết quả thanh toán
            var resultUrl = $"{_config["VnPay:PaymentBackReturnUrl"]}?success={response.Success}&orderId={response.OrderId}&transactionId={response.TransactionId}&responseCode={response.VnPayResponseCode}&description={response.OrderDescription}";

            return Redirect(resultUrl);
        }*/

    }

}
