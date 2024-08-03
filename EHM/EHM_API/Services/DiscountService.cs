using AutoMapper;
using EHM_API.DTOs.DiscountDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly EHMDBContext _context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;

        public DiscountService(EHMDBContext context, IInvoiceRepository invoiceRepository, IDiscountRepository discountRepository, IMapper mapper)
        {
            _context = context;
            _invoiceRepository = invoiceRepository;
            _discountRepository = discountRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DiscountAllDTO>> GetAllAsync()
        {
            var discounts = await _discountRepository.GetAllAsync();
            bool statusUpdated = false;

            foreach (var discount in discounts)
            {
                bool statusChanged = false;

                if (discount.EndTime.HasValue && DateTime.Now >= discount.EndTime.Value)
                {
                    discount.DiscountStatus = false;
                    statusChanged = true;
                }

           
                if (discount.QuantityLimit.HasValue)
                {
                    var invoiceCount = await _invoiceRepository.CountInvoicesByDiscountIdAsync(discount.DiscountId);

                    if (invoiceCount >= discount.QuantityLimit.Value)
                    {
                        discount.DiscountStatus = false;
                        statusChanged = true;
                    }
                }

               
                if (statusChanged)
                {
                    await _discountRepository.UpdateAsync(discount);
                    statusUpdated = true;
                }
            }

            return _mapper.Map<IEnumerable<DiscountAllDTO>>(discounts);
        }


        public async Task<IEnumerable<DiscountAllDTO>> GetByIdAsync(int id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            if (discount == null)
            {
                return null;
            }
            var similarDiscounts = await _discountRepository.GetDiscountsWithSimilarAttributesAsync(id);

            var allDiscounts = new List<Discount> { discount };
            allDiscounts.AddRange(similarDiscounts);

            return _mapper.Map<IEnumerable<DiscountAllDTO>>(allDiscounts);
        }

        public async Task<CreateDiscountResponse> AddAsync(CreateDiscount discountDto)
        {
            var discount = _mapper.Map<Discount>(discountDto);
            var addedDiscount = await _discountRepository.AddAsync(discount);
            return _mapper.Map<CreateDiscountResponse>(addedDiscount);
        }


        public async Task<CreateDiscount?> UpdateAsync(int id, CreateDiscount discountDto)
        {
            var existingDiscount = await _discountRepository.GetByIdAsync(id);
            if (existingDiscount == null)
            {
                return null;
            }

            _mapper.Map(discountDto, existingDiscount);
            await _discountRepository.UpdateAsync(existingDiscount);

            return _mapper.Map<CreateDiscount>(existingDiscount);
        }

        public async Task<IEnumerable<DiscountAllDTO>> SearchAsync(string keyword)
        {
            var discounts = await _discountRepository.SearchAsync(keyword);
            return _mapper.Map<IEnumerable<DiscountAllDTO>>(discounts);
        }
        public async Task<bool> UpdateDiscountStatusAsync()
        {
            var discounts = await _context.Discounts.ToListAsync();
            bool statusUpdated = false;

            foreach (var discount in discounts)
            {
                bool statusChanged = false;

              
                if (discount.EndTime.HasValue && DateTime.Now > discount.EndTime.Value)
                {
                    discount.DiscountStatus = false;
                    statusChanged = true;
                }

                if (discount.QuantityLimit.HasValue)
                {
                    var orderCount = await _discountRepository.CountOrdersInRangeAsync(discount.StartTime.Value, discount.EndTime.Value);

                    if (orderCount >= discount.QuantityLimit.Value)
                    {
                        discount.DiscountStatus = false;
                        statusChanged = true;
                    }
                }

                if (statusChanged)
                {
                    await _discountRepository.UpdateAsync(discount);
                    statusUpdated = true;
                }
            }

            return statusUpdated;
        }

        public async Task<IEnumerable<DiscountDTO>> GetActiveDiscountsAsync()
        {
            var discounts = await _discountRepository.GetActiveDiscountsAsync();
            return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
        }



    }
}
