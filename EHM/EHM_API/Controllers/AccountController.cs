using AutoMapper;
using EHM_API.DTOs.AccountDTO;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using EHM_API.Repositories;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IGoogleRepository _googleRepository;
        private readonly IGoogleService _googleService;
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;
        private readonly EHMDBContext _context;

        public AccountController(IAccountService accountService, IMapper mapper, IGoogleRepository googleRepository, IGoogleService googleService, IAccountRepository accountRepository, IEmailService emailService, EHMDBContext context)
        {
            _accountService = accountService;
            _mapper = mapper;
            _googleRepository = googleRepository;
            _googleService = googleService;
            _accountRepository = accountRepository;
            _emailService = emailService;
            _context = context;
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
                var accountDTOResponse = _mapper.Map<CreateAccountDTO>(account);
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


        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<GetAccountByRole>>> GetAccountsByRole(string role)
        {
            var accounts = await _accountService.GetAccountsByRoleAsync(role.ToLower());
            if (accounts == null || !accounts.Any())
            {
                return NotFound();
            }
            return Ok(accounts);
        }

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateAccountStatus(int id, [FromBody] bool isActive)
        {
            var result = await _accountService.UpdateAccountStatusAsync(id, isActive);
            if (!result)
            {
                return NotFound("Account not found");
            }

            return Ok("Account status updated successfully");
        }

		[HttpPut("{accountId}")]
		public async Task<IActionResult> UpdateProfile(int accountId, [FromBody] UpdateProfileDTO dto)
		{
			var result = await _accountService.UpdateProfileAsync(accountId, dto);
			if (!result)
			{
				return NotFound(new { message = "Không tìm thấy tài khoản" });
			}

			return Ok(new { message = "Cập nhật hồ sơ thành công" });
		}


        [HttpPut("changepassword/{accountId}")]
        public async Task<IActionResult> ChangePassword(int accountId, [FromBody] ChangePasswordDTO dto)
        {
            var result = await _accountService.ChangePasswordAsync(accountId, dto);

            if (!result)
            {
                return BadRequest(new { message = "Đổi mật khẩu không thành công. Vui lòng kiểm tra lại thông tin." });
            }

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }
        // PUT: api/Account/{id}/role
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleUpdateDTO roleUpdateDto)
        {
            if (string.IsNullOrEmpty(roleUpdateDto.Role))
            {
                return BadRequest("Role cannot be empty.");
            }

            try
            {
                var result = await _accountService.UpdateRoleAsync(id, roleUpdateDto);
                if (!result)
                {
                    return NotFound($"Account with ID {id} not found.");
                }
                return Ok($"Role updated to {roleUpdateDto.Role} for Account ID {id}");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
           
            var result = await _accountService.ForgotPasswordAsync(request);
            if (!result)
            {
                return Ok(new
                {
                    success = false,
                    message = "Email not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "A new password has been sent to your email."
            });
        }
        [HttpPost("verify")]
        public async Task<IActionResult> Verify(RegisterAccountDTO registerDto)
        {
            var errors = new Dictionary<string, string>();

            // Kiểm tra email rỗng hoặc định dạng sai
            if (string.IsNullOrWhiteSpace(registerDto.Email))
            {
                errors["Email"] = "Email không được để trống.";
            }
            else if (!new EmailAddressAttribute().IsValid(registerDto.Email)) // Kiểm tra định dạng email hợp lệ
            {
                errors["Email"] = "Email không hợp lệ.";
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(registerDto.Password) || registerDto.Password.Length < 6)
            {
                errors["Password"] = "Mật khẩu phải có độ dài ít nhất 6 ký tự.";
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                errors["ConfirmPassword"] = "Mật khẩu và Xác nhận mật khẩu không khớp.";
            }

            // Kiểm tra xem tài khoản đã tồn tại chưa
            var existingUsername = await _accountService.AccountExistsAsync(registerDto.Username);
            if (existingUsername)
            {
                errors["Username"] = "Tên tài khoản đã tồn tại.";
            }

            // Kiểm tra email đã tồn tại
            var existingEmail = await _accountService.EmailExistsAsync(registerDto.Email);
            if (existingEmail)
            {
                errors["Email"] = "Email đã tồn tại.";
            }

            // Nếu có lỗi, trả về BadRequest
            if (errors.Any())
            {
                return BadRequest(errors);
            }

            try
            {
                // Tạo tài khoản mới
                var newAccount = new CreateAccountDTO
                {
                    Username = registerDto.Username,
                    Password = registerDto.Password,
                    Email = registerDto.Email,
                    Role = "User", // Role mặc định
                    IsActive = true // Tài khoản kích hoạt ngay lập tức
                };



                // Tạo mã OTP (chỉ để hiển thị)
                var otp = _googleService.GenerateOTP(registerDto.Email);

                // Trả về thông báo tạo tài khoản thành công kèm OTP
                return Ok(new
                {
                 
                    otp = otp // OTP được trả về kèm với thông báo thành công
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.", details = ex.Message });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterAccountDTO registerDto)
        {
            var errors = new Dictionary<string, string>();

            // Kiểm tra email rỗng hoặc định dạng sai
            if (string.IsNullOrWhiteSpace(registerDto.Email))
            {
                errors["Email"] = "Email không được để trống.";
            }
            else if (!new EmailAddressAttribute().IsValid(registerDto.Email)) // Kiểm tra định dạng email hợp lệ
            {
                errors["Email"] = "Email không hợp lệ.";
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(registerDto.Password) || registerDto.Password.Length < 6)
            {
                errors["Password"] = "Mật khẩu phải có độ dài ít nhất 6 ký tự.";
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                errors["ConfirmPassword"] = "Mật khẩu và Xác nhận mật khẩu không khớp.";
            }

            // Kiểm tra xem tài khoản đã tồn tại chưa
            var existingUsername = await _accountService.AccountExistsAsync(registerDto.Username);
            if (existingUsername)
            {
                errors["Username"] = "Tên tài khoản đã tồn tại.";
            }

            // Kiểm tra email đã tồn tại
            var existingEmail = await _accountService.EmailExistsAsync(registerDto.Email);
            if (existingEmail)
            {
                errors["Email"] = "Email đã tồn tại.";
            }

            // Nếu có lỗi, trả về BadRequest
            if (errors.Any())
            {
                return BadRequest(errors);
            }

            try
            {
                // Tạo tài khoản mới
                var newAccount = new CreateAccountDTO
                {
                    Username = registerDto.Username,
                    Password = registerDto.Password,
                    Email = registerDto.Email,
                    Role = "User", // Role mặc định
                    IsActive = true // Tài khoản kích hoạt ngay lập tức
                };

                // Gọi service để lưu tài khoản vào database
                await _accountService.CreateAccountAsync(newAccount);

                // Tạo mã OTP (chỉ để hiển thị)
                var otp = _googleService.GenerateOTP(registerDto.Email);

                // Trả về thông báo tạo tài khoản thành công kèm OTP
                return Ok(new
                {
                    message = "Tài khoản đã được tạo thành công.",
                    otp = otp // OTP được trả về kèm với thông báo thành công
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.", details = ex.Message });
            }
        }
        [HttpGet("GetTypeNotificationByAccountId/{accountId}")]
        public async Task<IActionResult> GetRoleByAccountId(int accountId)
        {
            // Tìm account dựa trên AccountId
            var account = await _context.Accounts.FindAsync(accountId);

            // Kiểm tra xem account có tồn tại không
            if (account == null)
            {
                return NotFound("Account không tồn tại");
            }

            // Kiểm tra Role và trả về số tương ứng
            switch (account.Role)
            {
                case "User":
                    return Ok((int)AccountRole.User);
                case "OrderStaff":
                    return Ok((int)AccountRole.OrderStaff);
                case "Cashier":
                    return Ok((int)AccountRole.Cashier);
                case "Chef":
                    return Ok((int)AccountRole.Chef);
                default:
                    return BadRequest("Vai trò không hợp lệ");
            }
        }

    }



}
