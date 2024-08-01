using EHM_API.DTOs.SettingDTO.Manager;

namespace EHM_API.Services
{
    public interface ISettingService
    {
        Task<IEnumerable<SettingAllDTO>> GetAllAsync();
        Task<SettingAllDTO> AddAsync(SettingAllDTO settingDto);
        Task<SettingAllDTO> GetByIdAsync(int id);
        Task<SettingAllDTO> UpdateAsync(int id, SettingAllDTO settingDto);
        Task<bool> DeleteAsync(int id);
    }
}
