using EHM_API.DTOs.DiscountDTO.Manager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountAllDTO>> GetAllAsync();
        Task<object?> GetByIdAsync(int id);
        Task<CreateDiscountResponse> AddAsync(CreateDiscount discountDto);
        Task<CreateDiscount> UpdateAsync(int id, CreateDiscount discountDto);
        Task<IEnumerable<DiscountAllDTO>> SearchAsync(string keyword);
        Task<bool> UpdateDiscountStatusAsync();
        Task<IEnumerable<DiscountDTO>> GetActiveDiscountsAsync();
    }
}
