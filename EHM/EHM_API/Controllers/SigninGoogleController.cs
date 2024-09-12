using EHM_API.DTOs;
using EHM_API.DTOs.GoogleDTO;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ProjectSchedule.Authenticate;
using EHM_API.DTOs.Email;
namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IGoogleService _accountService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public GoogleAuthController(IGoogleService accountService, IHttpClientFactory httpClientFactory, HttpClient httpClient, JwtTokenGenerator jwtTokenGenerator)
        {
            _accountService = accountService;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleUserInfo request)
        {

            var account = _accountService.GetByEmail(request.Email);

            if (account != null)
            {
                var token = _jwtTokenGenerator.GenerateJwtToken(account);

                return Ok(new
                {
                    Message = "Tài khoản với email này đã tồn tại.",
                    token,
                    account.AccountId,
                    account.Username,
                    account.Role
                });
            }

            var newAccount = await _accountService.RegisterGoogleAccountAsync(request.Email);

            var newToken = _jwtTokenGenerator.GenerateJwtToken(newAccount);

            return Ok(new
            {
                Message = "Đăng ký thành công",
                token = newToken,
                newAccount.AccountId,
                newAccount.Username,
                newAccount.Role
            });
        }

        [HttpPost("Send-OTP")]
        [AllowAnonymous]
        public async Task<IActionResult> SendOTP([FromBody] GoogleUserInfo request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email không được để trống.");
            }

            var account = _accountService.GetByEmail(request.Email);

            if (account != null)
            {
             
                return Ok(new
                {
                    Message = "Tài khoản với email này đã tồn tại.",
                    account.Email,
                });
            }

            var otp = _accountService.GenerateOTP(request.Email);
            var sendOtpResult = await _accountService.SendOtpToEmail(request.Email, otp);

            if (!sendOtpResult)
            {
                return BadRequest("Gửi OTP thất bại.");
            }

            return Ok(new
            {
                Message = "OTP đã được gửi đến email của bạn. Vui lòng xác nhận OTP.",
                Otp = otp 
            });
        }


        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest("Email và OTP không được để trống.");
            }

            var isOtpValid = _accountService.VerifyOtp(request.Email, request.Otp);
            if (!isOtpValid)
            {
                return BadRequest("OTP không hợp lệ.");
            }

            var newAccount = await _accountService.RegisterGoogleAccountAsync(request.Email);
            var newToken = _jwtTokenGenerator.GenerateJwtToken(newAccount);

            return Ok(new
            {
                Message = "Đăng ký thành công",
                token = newToken,
                newAccount.AccountId,
                newAccount.Username,
                newAccount.Role
            });
        }


        [HttpPost("login-with-credentials")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var account = _accountService.GetByEmail(request.Email);

            if (account == null || !VerifyPassword(request.Password, account.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _accountService.GenerateJwtToken(account);
            return Ok(new LoginResponse { Token = token });
        }

        private bool VerifyPassword(string password, string storedPassword)
        {
            return password == storedPassword; 
        }

        private async Task<GoogleUserInfo> GetGoogleUserInfoAsync(string tokenId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={tokenId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error from Google API: {response.StatusCode} - {errorContent}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(responseBody);
                return userInfo;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return null;
            }
        }
        [HttpPut("update-password")]
        public IActionResult UpdatePassword([FromBody] UpdatePasswordDTO dto)
        { 
            if(dto.NewPassword.Length < 6 || dto.NewPassword.Length > 32)
            {
                return BadRequest("Password length 6 -> 32");
            }
            if (!_accountService.UpdatePassword(dto))
            {
                return NotFound("Account not found or update failed.");
            }

            return Ok("Password updated successfully.");
        }
    }
}
