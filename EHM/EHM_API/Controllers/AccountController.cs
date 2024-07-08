using EHM_API.DTOs.AccountDTO;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IAccountService _accountService;

		public AccountController(IAccountService accountService)
		{
			_accountService = accountService;
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDTO accountDTO)
		{
			var errors = new Dictionary<string, string>();

			if (accountDTO == null)
			{
				return BadRequest(new { message = "Thông tin tài khoản không được để trống." });
			}

			if (string.IsNullOrWhiteSpace(accountDTO.FirstName))
			{
				errors["FirstName"] = "Họ không được bỏ trống.";
			}
			else if (!System.Text.RegularExpressions.Regex.IsMatch(accountDTO.FirstName, "^[a-zA-Z0-9 ]*$"))
			{
				errors["FirstName"] = "Họ không được chứa ký tự đặc biệt.";
			}

			if (string.IsNullOrWhiteSpace(accountDTO.LastName))
			{
				errors["LastName"] = "Tên không được bỏ trống.";
			}
			else if (!System.Text.RegularExpressions.Regex.IsMatch(accountDTO.LastName, "^[a-zA-Z0-9 ]*$"))
			{
				errors["LastName"] = "Tên không được chứa ký tự đặc biệt.";
			}

			if (string.IsNullOrWhiteSpace(accountDTO.Email))
			{
				errors["Email"] = "Email không được bỏ trống.";
			}
			else if (!new EmailAddressAttribute().IsValid(accountDTO.Email))
			{
				errors["Email"] = "Email không hợp lệ.";
			}

			if (string.IsNullOrWhiteSpace(accountDTO.Username))
			{
				errors["Username"] = "Tên người dùng không được bỏ trống.";
			}
			else if (accountDTO.Username.Length > 20)
			{
				errors["Username"] = "Tên người dùng không được vượt quá 20 ký tự.";
			}

			if (string.IsNullOrWhiteSpace(accountDTO.Password))
			{
				errors["Password"] = "Mật khẩu không được bỏ trống.";
			}
			else if (accountDTO.Password.Length < 6 || accountDTO.Password.Length > 50)
			{
				errors["Password"] = "Mật khẩu phải có độ dài từ 6 đến 50 ký tự.";
			}

			if (string.IsNullOrWhiteSpace(accountDTO.Role))
			{
				errors["Role"] = "Vai trò không được bỏ trống.";
			}

			if (string.IsNullOrWhiteSpace(accountDTO.Address))
			{
				errors["Address"] = "Địa chỉ không được bỏ trống.";
			}
			else if (System.Text.RegularExpressions.Regex.IsMatch(accountDTO.Address, @"[@!$%^&*()]"))
			{
				errors["Address"] = "Địa chỉ không được chứa các ký tự đặc biệt.";
			}

			if (string.IsNullOrWhiteSpace(accountDTO.Phone))
			{
				errors["Phone"] = "Số điện thoại không được bỏ trống.";
			}
			else if (!System.Text.RegularExpressions.Regex.IsMatch(accountDTO.Phone, @"^\d{10,15}$"))
			{
				errors["Phone"] = "Số điện thoại phải chứa từ 10 đến 15 chữ số.";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}
			try
			{
				var accountExists = await _accountService.AccountExistsAsync(accountDTO.Username);
				if (accountExists)
				{
					return Conflict(new { message = "Tên tài khoản đã tồn tại." });
				}

				var account = await _accountService.CreateAccountAsync(accountDTO);
				return Ok(new { message = "Tài khoản được tạo thành công."});
			}
			catch (Exception ex)	
			{
				return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.", details = ex.Message });
			}
		}
	}
}
