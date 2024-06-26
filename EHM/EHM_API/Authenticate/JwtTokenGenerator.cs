﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProjectSchedule.Models;
using EHM_API.Models;

namespace ProjectSchedule.Authenticate
{
    public class JwtTokenGenerator
    {
        private readonly JwtSetting _jwtSettings;

        public JwtTokenGenerator(JwtSetting jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateJwtToken(Account ac)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
              {
        new Claim(ClaimTypes.NameIdentifier, ac.AccountId.ToString()),
        new Claim(ClaimTypes.Name, ac.Username),
        new Claim(ClaimTypes.Role, ac.Role),
                  // Add custom claims as needed
              }),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}