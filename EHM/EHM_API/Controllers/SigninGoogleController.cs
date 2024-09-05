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
namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IGoogleService _accountService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public GoogleAuthController(IGoogleService accountService, IHttpClientFactory httpClientFactory, System.Net.Http.HttpClient httpClient)
        {
            _accountService = accountService;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleUserInfo request)
        {
            
            var account = _accountService.GetByEmail(request.Email);
            if (account != null)
            {
                return BadRequest("An account with this email already exists.");
            }


            var newAccount = await _accountService.RegisterGoogleAccountAsync(request);


            var token = _accountService.GenerateJwtToken(newAccount);

            return Ok(new { Message = "Registration successful.", Token = token });
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
                // Log the exception and handle it appropriately
                Console.WriteLine($"Request error: {ex.Message}");
                return null;
            }
        }
     
    }
}
