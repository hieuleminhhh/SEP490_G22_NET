using AutoMapper;
using EHM_API.DTOs.AccountDTO;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
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

            if (!string.IsNullOrWhiteSpace(accountDTO.Password))
            {
                existingAccount.Password = BCrypt.Net.BCrypt.HashPassword(accountDTO.Password);
            }

            var updatedAccount = await _accountRepository.UpdateAccountAsync(existingAccount);
            return _mapper.Map<UpdateAccountDTO>(updatedAccount);
        }

        // In AccountService.cs
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

    }
}
