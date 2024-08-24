using EHM_API.DTOs.HomeDTO;
using EHM_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ProjectSchedule.Authenticate;

namespace EHM_API.Controllers
{
    [ApiController]
    [Route("api")]
    public class HomeController : Controller
    {
        private readonly EHMDBContext _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        public HomeController(EHMDBContext context, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }


		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] LoginDTO model)
		{
			if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
			{
				return BadRequest(new { Message = "Không được để trống, người dùng cần nhập đầy đủ username và password" });
			}

			var st = await _context.Accounts
				.Where(t => t.Username == model.Username && t.Password == model.Password)
				.FirstOrDefaultAsync();

			if (st == null)
			{
				return Unauthorized(new { Message = "Username và password không hợp lệ" });
			}

			if (st.IsActive == false)
			{
				return Unauthorized(new { Message = "Tài khoản của bạn hiện đang bị khóa. Vui lòng liên hệ với quản trị viên để được hỗ trợ." });
			}

			var token = _jwtTokenGenerator.GenerateJwtToken(st);

			return Ok(new
			{
				Message = "Đăng nhập thành công",
				token,
				st.AccountId,
				st.Username,
				st.Role
			});
		}



	}
}
