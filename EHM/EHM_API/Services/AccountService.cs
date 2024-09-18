using AutoMapper;
using EHM_API.DTOs.AccountDTO;
using EHM_API.DTOs.Email;
using EHM_API.Models;
using EHM_API.Repositories;
using System.Net.Mail;
using System.Net;

namespace EHM_API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<CreateAccountDTO> CreateAccountAsync(CreateAccountDTO accountDTO)
		{
			var account = _mapper.Map<Account>(accountDTO);

			var createdAccount = await _accountRepository.AddAccountAsync(account);

			var createdAccountDTO = _mapper.Map<CreateAccountDTO>(createdAccount);

			return createdAccountDTO;
		}


		public async Task<bool> AccountExistsAsync(string username)
        {
            return await _accountRepository.AccountExistsAsync(username);
        }

        public async Task<IEnumerable<GetAccountDTO>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAccountsAsync();
            return _mapper.Map<IEnumerable<GetAccountDTO>>(accounts);
        }

        public async Task<GetAccountDTO> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);
            return account == null ? null : _mapper.Map<GetAccountDTO>(account);
        }

        public async Task<UpdateAccountDTO> UpdateAccountAsync(int id, UpdateAccountDTO accountDTO)
        {
            var existingAccount = await _accountRepository.GetAccountByIdAsync(id);
            if (existingAccount == null)
            {
                return null;
            }

            _mapper.Map(accountDTO, existingAccount);

            // Không mã hóa mật khẩu, lưu trực tiếp mật khẩu thô
            if (!string.IsNullOrWhiteSpace(accountDTO.Password))
            {
                existingAccount.Password = accountDTO.Password;
            }

            var updatedAccount = await _accountRepository.UpdateAccountAsync(existingAccount);
            return _mapper.Map<UpdateAccountDTO>(updatedAccount);
        }


        public async Task<bool> RemoveAccountAsync(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);
            if (account == null)
            {
                return false;
            }

            await _accountRepository.RemoveAccountAsync(id);
            return true;
        }
        public async Task<IEnumerable<GetAccountByRole>> GetAccountsByRoleAsync(string role)
        {
            var accounts = await _accountRepository.GetAccountsByRoleAsync(role.ToLower());
            return _mapper.Map<IEnumerable<GetAccountByRole>>(accounts);
        }
        public async Task<bool> UpdateAccountStatusAsync(int id, bool isActive)
        {
            return await _accountRepository.UpdateAccountStatusAsync(id, isActive);
        }

		public async Task<bool> UpdateProfileAsync(int accountId, UpdateProfileDTO dto)
		{
			var account = await _accountRepository.GetAccountByIdAsync(accountId);
			if (account == null)
			{
				return false;
			}

			_mapper.Map(dto, account);

			return await _accountRepository.UpdateProfileAccount(account);
		}


        public async Task<bool> ChangePasswordAsync(int accountId, ChangePasswordDTO dto)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);

            if (account == null)
            {
                return false;
            }

           
            if (account.Password == null)
            {
              
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return false; 
                }

                account.Password = dto.NewPassword;
            }
            else
            {
               
                if (account.Password != dto.CurrentPassword)
                {
                    return false;
                }

                
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return false; 
                }

             
                account.Password = dto.NewPassword;
            }

            return await _accountRepository.UpdateProfileAccount(account);
        }
        public async Task<bool> UpdateRoleAsync(int accountId, RoleUpdateDTO roleUpdateDto)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return false;  
            }

         
            account.Role = roleUpdateDto.Role;

            await _accountRepository.SaveAsync();  
            return true;
        }
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO request)
        {
            var account = _accountRepository.GetByEmail(request.Email);
            if (account == null)
            {
                return false; 
            }

            string newPassword = PasswordGenerator.GeneratePassword(); 

            await _accountRepository.UpdatePasswordByEmailAsync(request.Email, newPassword);

            var emailDto = new SendEmailRequestDTO
            {
                ToEmail = request.Email,
                Subject = "Mật khẩu mới",
                Body = $"Mật khẩu mới của bạn là: {newPassword}"
            };
            await _emailService.SendEmailAsync(emailDto.ToEmail, emailDto.Subject, emailDto.Body);

            return true;
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            // Gọi repository hoặc context để kiểm tra xem email đã tồn tại chưa
            var account = await _accountRepository.GetAccountByEmailAsync(email);
            return account != null;
        }
    }
}
