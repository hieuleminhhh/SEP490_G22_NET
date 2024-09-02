using EHM_API.DTOs;
using EHM_API.DTOs.GoogleDTO;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IGoogleService _accountService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleAuthController(IGoogleService accountService, IHttpClientFactory httpClientFactory)
        {
            _accountService = accountService;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Logs in using a Google token and returns a JWT token if the account exists.
        /// If the account does not exist, prompts for registration.
        /// </summary>
        /// <param name="request">Google login request containing the token ID.</param>
        /// <returns>JWT token if the account exists, otherwise a registration prompt.</returns>
        [HttpPost("login")]
/*        [AllowAnonymous]*/
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var userInfo = await GetGoogleUserInfoAsync(request.TokenId);

            if (userInfo == null)
            {
                return BadRequest("Invalid Google token.");
            }

            var account = _accountService.GetByEmail(userInfo.Email);

            if (account == null)
            {
                // Account does not exist, prompt registration
                return Ok(new { Message = "Account not found. Please register.", Email = userInfo.Email });
            }
            else
            {
                // Generate JWT token
                var token = _accountService.GenerateJwtToken(account);
                return Ok(new { Token = token });
            }
        }


        /// <summary>
        /// Logs in using email and password and returns a JWT token if the credentials are valid.
        /// </summary>
        /// <param name="request">Login request containing email and password.</param>
        /// <returns>JWT token if the credentials are valid, otherwise an Unauthorized response.</returns>
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
            // Implement password verification logic here
            // Use a secure method for hashing and comparing passwords
            return password == storedPassword; // Update this to use hashed passwords
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
