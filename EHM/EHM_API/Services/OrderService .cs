    using AutoMapper;
    using EHM_API.DTOs.DishDTO;
    using EHM_API.DTOs.OrderDTO;
    using EHM_API.Models;
    using EHM_API.Repositories;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    namespace EHM_API.Services
    {
        public class OrderService : IOrderService
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMapper _mapper;
            private readonly EHMDBContext _context;

            public OrderService(IOrderRepository orderRepository, IMapper mapper, EHMDBContext context)
            {
                _orderRepository = orderRepository;
                _mapper = mapper;
                _context = context;
            }

            public async Task<IEnumerable<OrderDTOAll>> GetAllOrdersAsync()
            {
                var orders = await _orderRepository.GetAllAsync();
                var orderDtos = _mapper.Map<IEnumerable<OrderDTOAll>>(orders);
                return orderDtos;
            }

		public async Task<IEnumerable<SearchPhoneOrderDTO>> GetAllOrdersToSearchAsync()
		{
			var orders = await _orderRepository.GetAllAsync();
			var orderDtos = _mapper.Map<IEnumerable<SearchPhoneOrderDTO>>(orders);
			return orderDtos;
		}

		public async Task<OrderDTOAll> GetOrderByIdAsync(int id)
            {

                 var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return null;
                }
                return _mapper.Map<OrderDTOAll>(order);
            }

		public async Task<IEnumerable<SearchPhoneOrderDTO>> SearchOrdersAsync(string guestPhone = null)
		{
			if (string.IsNullOrWhiteSpace(guestPhone))
			{
				return await GetAllOrdersToSearchAsync();
			}

			var orders = await _orderRepository.SearchAsync(guestPhone);
			var orderDtos = _mapper.Map<IEnumerable<SearchPhoneOrderDTO>>(orders);
			return orderDtos;
		}





		public async Task<OrderDTOAll> CreateOrderAsync(CreateOrderDTO createOrderDto)
            {

                var order = _mapper.Map<Order>(createOrderDto);

            
                var createdOrder = await _orderRepository.AddAsync(order);

                var orderDto = _mapper.Map<OrderDTOAll>(createdOrder);

                return orderDto;
            }

            public async Task<OrderDTOAll> UpdateOrderAsync(int id, UpdateOrderDTO updateOrderDto)
            {
                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                {
                    return null;
                }

                _mapper.Map(updateOrderDto, existingOrder);

                var updatedOrder = await _orderRepository.UpdateAsync(existingOrder);
                return _mapper.Map<OrderDTOAll>(updatedOrder);
            }

            public async Task<bool> DeleteOrderAsync(int id)
            {
                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                {
                    return false;
                }

                var orderDetails = _context.OrderDetails.Where(od => od.OrderId == id);
                _context.OrderDetails.RemoveRange(orderDetails);
                var result = await _orderRepository.DeleteAsync(id);

                await _context.SaveChangesAsync();

                return result;
            }

		public async Task<bool> CancelOrderAsync(int orderId)
		{
			var existingOrder = await _orderRepository.GetByIdAsync(orderId);
			if (existingOrder == null)
			{
				return false;
			}

			if (existingOrder.Status != 0)
			{
				return false;
			}

			existingOrder.Status = 4; 
			await _orderRepository.UpdateAsync(existingOrder);
			return true;
		}

	}
    }
