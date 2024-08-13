using AutoMapper;
using EHM_API.DTOs.AccountDTO;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                var accountDTOs = _mapper.Map<IEnumerable<GetAccountDTO>>(accounts);
                return Ok(accountDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.", details = ex.Message });
            }
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Tài khoản không tồn tại." });
                }
                var accountDTO = _mapper.Map<GetAccountDTO>(account);
                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.", details = ex.Message });
            }
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
            else if (!Regex.IsMatch(accountDTO.FirstName, @"^[\p{L}\p{M}\p{N} ]*$"))
            {
                errors["FirstName"] = "Họ không được chứa ký tự đặc biệt.";
            }

            if (string.IsNullOrWhiteSpace(accountDTO.LastName))
            {
                errors["LastName"] = "Tên không được bỏ trống.";
            }
            else if (!Regex.IsMatch(accountDTO.LastName, @"^[\p{L}\p{M}\p{N} ]*$"))
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
                var accountDTOResponse = _mapper.Map<GetAccountDTO>(account);
                return Ok(new { message = "Tài khoản được tạo thành công.", account = accountDTOResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.", details = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDTO accountDTO)
        {
            var errors = new Dictionary<string, string>();

            // Kiểm tra các lỗi tương tự như trong phương thức CreateAccount
            if (accountDTO == null)
            {
                return BadRequest(new { message = "Thông tin tài khoản không được để trống." });
            }

            if (string.IsNullOrWhiteSpace(accountDTO.FirstName))
            {
                errors["FirstName"] = "Họ không được bỏ trống.";
            }
            else if (!Regex.IsMatch(accountDTO.FirstName, @"^[\p{L}\p{M}\p{N} ]*$"))
            {
                errors["FirstName"] = "Họ không được chứa ký tự đặc biệt.";
            }

            if (string.IsNullOrWhiteSpace(accountDTO.LastName))
            {
                errors["LastName"] = "Tên không được bỏ trống.";
            }
            else if (!Regex.IsMatch(accountDTO.LastName, @"^[\p{L}\p{M}\p{N} ]*$"))
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

            if (!string.IsNullOrWhiteSpace(accountDTO.Password) &&
                (accountDTO.Password.Length < 6 || accountDTO.Password.Length > 50))
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
                var account = await _accountService.UpdateAccountAsync(id, accountDTO);
                if (account == null)
                {
                    return NotFound(new { message = "Tài khoản không tồn tại." });
                }
                var accountDTOResponse = _mapper.Map<GetAccountDTO>(account);
                return Ok(new { message = "Tài khoản được cập nhật thành công.", account = accountDTOResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.", details = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var success = await _accountService.RemoveAccountAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Tài khoản không tồn tại." });
                }
                return Ok(new { message = "Tài khoản được xóa thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.", details = ex.Message });
            }
        }
    }
}
