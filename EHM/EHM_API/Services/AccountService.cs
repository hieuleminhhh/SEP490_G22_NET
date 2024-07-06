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

		public async Task<Account> CreateAccountAsync(CreateAccountDTO accountDTO)
		{
			var account = _mapper.Map<Account>(accountDTO);
			return await _accountRepository.AddAccountAsync(account);
		}

		public async Task<bool> AccountExistsAsync(string username)
		{
			return await _accountRepository.AccountExistsAsync(username);
		}
	}
}
