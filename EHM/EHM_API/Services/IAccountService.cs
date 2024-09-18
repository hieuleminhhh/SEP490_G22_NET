using EHM_API.DTOs.AccountDTO;
using EHM_API.DTOs.Email;
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
        Task<IEnumerable<GetAccountByRole>> GetAccountsByRoleAsync(string role);
        Task<bool> UpdateAccountStatusAsync(int id, bool isActive);

        Task<bool> UpdateProfileAsync(int accountId, UpdateProfileDTO dto);
        Task<bool> ChangePasswordAsync(int accountId, ChangePasswordDTO dto);
        Task<bool> UpdateRoleAsync(int accountId, RoleUpdateDTO roleUpdateDto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO request);
        Task<bool> EmailExistsAsync(string email);
    }
}
