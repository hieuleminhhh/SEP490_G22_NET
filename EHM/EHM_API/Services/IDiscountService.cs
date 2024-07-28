using EHM_API.DTOs.DiscountDTO.Manager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountAllDTO>> GetAllAsync();
        Task<DiscountAllDTO> GetByIdAsync(int id);
        Task<DiscountAllDTO> AddAsync(DiscountAllDTO discountDto);
        Task<DiscountAllDTO> UpdateAsync(int id, DiscountAllDTO discountDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<DiscountAllDTO>> SearchAsync(string keyword);
    }
}
