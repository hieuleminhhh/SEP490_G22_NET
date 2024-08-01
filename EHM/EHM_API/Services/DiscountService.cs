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
            return _mapper.Map<IEnumerable<DiscountAllDTO>>(discounts);
        }

        public async Task<DiscountAllDTO> GetByIdAsync(int id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            return _mapper.Map<DiscountAllDTO>(discount);
        }

        public async Task<DiscountAllDTO> AddAsync(DiscountAllDTO discountDto)
        {
            var discount = _mapper.Map<Discount>(discountDto);
            var addedDiscount = await _discountRepository.AddAsync(discount);
            return _mapper.Map<DiscountAllDTO>(addedDiscount);
        }

        public async Task<DiscountAllDTO?> UpdateAsync(int id, DiscountAllDTO discountDto)
        {
            var existingDiscount = await _discountRepository.GetByIdAsync(id);
            if (existingDiscount == null)
            {
                return null;
            }

            _mapper.Map(discountDto, existingDiscount);
            await _discountRepository.UpdateAsync(existingDiscount);

            return _mapper.Map<DiscountAllDTO>(existingDiscount);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _discountRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<DiscountAllDTO>> SearchAsync(string keyword)
        {
            var discounts = await _discountRepository.SearchAsync(keyword);
            return _mapper.Map<IEnumerable<DiscountAllDTO>>(discounts);
        }
        public async Task<bool> ApplyDiscountAsync(ApplyDiscountRequest request)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(request.InvoiceId);
            if (invoice == null)
            {
                return false; 
            }

            
            var discount = await _discountRepository.GetDiscountByIdAsync(request.DiscountId);
            if (discount == null || discount.DiscountStatus != true || discount.Type != 1)
            {
                return false;
            }


            var hasOrdersInRange = await _context.Orders
        .AnyAsync(o => o.OrderDate >= discount.StartTime &&
                       o.OrderDate <= discount.EndTime);

            if (!hasOrdersInRange)
            {
                return false; 
            }


            var orderCount = await _discountRepository.CountOrdersInRangeAsync(discount.StartTime.Value, discount.EndTime.Value);
            if (orderCount >= discount.QuantityLimit)
            {
                return false;
            }

           
            invoice.DiscountId = discount.DiscountId;
            await _invoiceRepository.UpdateInvoiceAsync(invoice);

            return true; 
        }

    }
}
