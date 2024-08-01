using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs;
using EHM_API.DTOs.SettingDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IMapper _mapper;

        public SettingService(ISettingRepository settingRepository, IMapper mapper)
        {
            _settingRepository = settingRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SettingAllDTO>> GetAllAsync()
        {
            var settings = await _settingRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SettingAllDTO>>(settings);
        }

        public async Task<SettingAllDTO> AddAsync(SettingAllDTO settingDto)
        {
            var setting = _mapper.Map<Setting>(settingDto);
            var addedSetting = await _settingRepository.AddAsync(setting);
            return _mapper.Map<SettingAllDTO>(addedSetting);
        }
        public async Task<SettingAllDTO> GetByIdAsync(int id)
        {
            var setting = await _settingRepository.GetByIdAsync(id);
            return _mapper.Map<SettingAllDTO>(setting);
        }

        public async Task<SettingAllDTO> UpdateAsync(int id, SettingAllDTO settingDto)
        {
            var setting = _mapper.Map<Setting>(settingDto);
            setting.Id = id;
            var updatedSetting = await _settingRepository.UpdateAsync(setting);
            return updatedSetting != null ? _mapper.Map<SettingAllDTO>(updatedSetting) : null;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _settingRepository.DeleteAsync(id);
        }
    }
}