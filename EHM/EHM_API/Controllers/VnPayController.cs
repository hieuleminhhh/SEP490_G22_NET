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
        public IActionResult GetVnPay()
        {
            try
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = 300000,
                    CreatedDate = DateTime.Now,
                    Description = "mua hang online",
                    FullName = "Lê Văn Dương",
                    OrderId = new Random().Next(1000, 100000)
                };

                // Get the VNPay URL
                var vnPayUrl = _vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel);
                Console.WriteLine($"VNPay URL: {vnPayUrl}");

                // Redirect the client to the VNPay URL
                return Ok(vnPayUrl);
            }
            catch (Exception ex)
            {
                // Log the exception to the console
                Console.WriteLine($"Error occurred: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Handle the exception and return an error message
                return BadRequest(new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }



    }
}
