using EHM_API.DTOs.AccountDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IAccountService
    {
        Task<CreateAccountDTO> CreateAccountAsync(CreateAccountDTO accountDTO);

        Task<bool> AccountExistsAsync(string username);

        Task<IEnumerable<GetAccountDTO>> GetAllAccountsAsync();

        Task<GetAccountDTO> GetAccountByIdAsync(int id);

        Task<UpdateAccountDTO> UpdateAccountAsync(int id, UpdateAccountDTO accountDTO);

        Task<bool> RemoveAccountAsync(int id);
    }
}
