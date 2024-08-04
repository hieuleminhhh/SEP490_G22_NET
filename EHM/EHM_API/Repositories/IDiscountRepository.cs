﻿using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<Discount>> GetAllAsync();
        Task<Discount> GetByIdAsync(int discountId);
        Task<Discount> AddAsync(Discount discount);
        Task<Discount> UpdateAsync(Discount discount);
        Task<IEnumerable<Discount>> SearchAsync(string keyword);
        Task<Discount> GetDiscountByIdAsync(int discountId);
        Task<int> CountOrdersInRangeAsync(DateTime startTime, DateTime endTime);
        Task<IEnumerable<Discount>> GetActiveDiscountsAsync();
        Task<IEnumerable<Discount>> GetDiscountsWithSimilarAttributesAsync(int discountId);
    }
}
