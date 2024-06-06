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
                return BadRequest(new { Message = "Invalid request. Username and password must be provided." });
            }
            var st = await _context.Accounts
                .Where(t => t.Username == model.Username && t.Password == model.Password)
                .FirstOrDefaultAsync();

            if (st == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            var token = _jwtTokenGenerator.GenerateJwtToken(st);

            return Ok(new
            {
                Message = "Login successful",
                token,
                st.AccountId,
                st.Username,
                st.Role
            });
        }


    }
}
