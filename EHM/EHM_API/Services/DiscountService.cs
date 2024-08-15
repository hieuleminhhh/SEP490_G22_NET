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
		private readonly IOrderRepository _orderRepository;
		private readonly IDiscountRepository _discountRepository;
		private readonly IMapper _mapper;

		public DiscountService(EHMDBContext context, IOrderRepository orderRepository, IDiscountRepository discountRepository, IMapper mapper)
		{
			_context = context;
			_orderRepository = orderRepository;
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


				if (discount.EndTime.HasValue && DateTime.Now.Date > discount.EndTime.Value.Date)
				{
					discount.DiscountStatus = false;
					statusChanged = true;
				}

				if (statusChanged == true)
				{
					await _discountRepository.UpdateAsync(discount);
					statusUpdated = true;
				}
			}

			return _mapper.Map<IEnumerable<DiscountAllDTO>>(discounts);
		}



		public async Task<object?> GetByIdAsync(int id)
		{
			var discount = await _discountRepository.GetByIdAsync(id);
			if (discount == null)
			{
				return null;
			}

			var similarDiscounts = await _discountRepository.GetDiscountsWithSimilarAttributesAsync(id);
			var allDiscounts = new List<Discount> { discount };
			allDiscounts.AddRange(similarDiscounts);

			if (discount.Type == 2)
			{
				return _mapper.Map<IEnumerable<DiscountWithDishesDTO>>(allDiscounts);
			}
			else if (discount.Type == 1)
			{
				return _mapper.Map<IEnumerable<DiscountAllDTO>>(allDiscounts);
			}

			return null;
		}

        public async Task<CreateDiscountResponse> AddAsync(CreateDiscount discountDto)
        {
        
            var discount = _mapper.Map<Discount>(discountDto);

            
            if (discount.StartTime.Value.Date == discount.EndTime.Value.Date)
            {
                discount.StartTime = DateTime.Now;
                discount.EndTime = discount.StartTime.Value.Date.AddDays(1).AddTicks(-1); 
            }

           
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

		public async Task<IEnumerable<DiscountDTO>> GetActiveDiscountsAsync()
		{
			var discounts = await _discountRepository.GetActiveDiscountsAsync();
			var discountDTOs = _mapper.Map<IEnumerable<DiscountDTO>>(discounts);

			foreach (var discountDTO in discountDTOs)
			{

				discountDTO.UsedCount = await _orderRepository.CountOrderByDiscountIdAsync(discountDTO.DiscountId);
			}

			return discountDTOs;
		}

		public async Task<GetDiscountByOrderID> GetDiscountByOrderIdAsync(int orderId)
		{
			var discount = await _discountRepository.GetDiscountByOrderIdAsync(orderId);
			return _mapper.Map<GetDiscountByOrderID>(discount);
		}

		public async Task<bool> IsDiscountNameExistingAsync(string discountName)
		{
			return await _discountRepository.DiscountNameExistsAsync(discountName);
		}
	}
}