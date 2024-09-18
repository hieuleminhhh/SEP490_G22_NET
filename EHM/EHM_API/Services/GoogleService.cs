using EHM_API.DTOs.Email;
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
        private readonly Dictionary<string, string> _otpStore = new Dictionary<string, string>();
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
                    new Claim(ClaimTypes.Email, account.Email)

                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<Account> RegisterGoogleAccountAsync(string email)
        {
            var existingAccount = _accountRepository.GetByEmail(email);
            if (existingAccount != null)
            {
                throw new InvalidOperationException("An account with this email already exists.");
            }

            var newAccount = new Account
            {
                Email = email,
                Username = email,
                IsActive = true,
                Role = "User"
            };

            await _accountRepository.AddAsync(newAccount);
            return newAccount;
        }
        public bool UpdatePassword(UpdatePasswordDTO dto)
        {
            var account = _accountRepository.GetAccountById(dto.AccountId);
            if (account == null)
            {
                return false; 
            }

            _accountRepository.UpdatePassword(dto.AccountId, dto.NewPassword);
            return true;
        }
        public string GenerateOTP(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _otpStore[email] = otp;
            Console.WriteLine($"Generated OTP for {email}: {otp}"); 
            return otp;
        }




        public async Task<bool> SendOtpToEmail(string email, string otp)
        {
            var emailDto = new SendEmailRequestDTO
            {
                ToEmail = email,
                Subject = "Xác thực OTP",
                Body = $"Mã OTP của bạn là {otp}"
            };
            return await _accountRepository.SendEmailAsync(emailDto);
        }


        public bool VerifyOtp(string email, string otp)
        {
            otp = otp.Trim();
            if (_otpStore.ContainsKey(email))
            {
                var storedOtp = _otpStore[email].Trim();
                if (storedOtp == otp)
                {
                    _otpStore.Remove(email); 
                    Console.WriteLine($"OTP verified for {email}: {otp}"); 
                    return true;
                }
                else
                {
                    Console.WriteLine($"OTP mismatch for {email}. Expected: {storedOtp}, Provided: {otp}"); 
                }
            }
            else
            {
                Console.WriteLine($"No OTP found for {email}"); 
            }
            return false;
        }


    }
}
