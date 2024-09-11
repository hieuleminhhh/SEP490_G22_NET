using EHM_API.DTOs.Email;
using EHM_API.Repositories;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;

        public EmailController(IAccountRepository accountRepository, IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequestDTO emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest.ToEmail))
            {
                return BadRequest(new { message = "Địa chỉ email không được để trống." });
            }

            await _emailService.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body);

            return Ok(new { message = "Email đã được gửi thành công." });
        }
    }
}
