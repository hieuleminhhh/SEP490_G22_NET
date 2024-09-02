using EHM_API.DTOs.GoogleDTO;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly IGoogleRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public GoogleService(IGoogleRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        public Account GetByEmail(string email)
        {
            return _accountRepository.GetByEmail(email);
        }

        public string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimTypes.Name, account.FirstName + " " + account.LastName),
                    // Add more claims if needed
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<Account> RegisterGoogleAccountAsync(GoogleUserInfo userInfo)
        {
            var newAccount = new Account
            {
                Email = userInfo.Email,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                IsActive = true,
                Role = "User"
            };

            await _accountRepository.AddAsync(newAccount);
            return newAccount;
        }
    }
}
