using Microsoft.IdentityModel.Tokens;
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

			// Kiểm tra và đảm bảo rằng Secret có độ dài tối thiểu là 32 bytes
			if (Encoding.UTF8.GetByteCount(_jwtSettings.Secret) < 32)
			{
				throw new ArgumentException("Secret key must be at least 256 bits (32 bytes) in length.");
			}
		}

        public string GenerateJwtToken(Account ac)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            // Kiểm tra và đảm bảo các giá trị không null
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, ac.AccountId.ToString()), // Bắt buộc AccountId phải có giá trị
    };

            // Kiểm tra nếu Username không null thì thêm vào claim
            if (!string.IsNullOrEmpty(ac.Username))
            {
                claims.Add(new Claim(ClaimTypes.Name, ac.Username));
            }

            // Kiểm tra nếu Role không null thì thêm vào claim
            if (!string.IsNullOrEmpty(ac.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, ac.Role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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
