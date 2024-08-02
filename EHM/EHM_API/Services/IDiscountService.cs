using EHM_API.DTOs.DiscountDTO.Manager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountAllDTO>> GetAllAsync();
        Task<DiscountAllDTO> GetByIdAsync(int id);
        Task<CreateDiscount> AddAsync(CreateDiscount discountDto);
        Task<CreateDiscount> UpdateAsync(int id, CreateDiscount discountDto);
        Task<IEnumerable<DiscountAllDTO>> SearchAsync(string keyword);
        Task<bool> ApplyDiscountAsync(ApplyDiscountRequest request);

        Task<IEnumerable<DiscountDTO>> GetActiveDiscountsAsync();

	}
}
