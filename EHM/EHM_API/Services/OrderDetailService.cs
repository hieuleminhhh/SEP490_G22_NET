using AutoMapper;
using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

      
        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IMapper mapper)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
        }
        public async Task<bool> UpdateOrderDetailQuantityAsync(int orderDetailId, int quantity)
        {
            var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(orderDetailId);
            if (orderDetail == null)
            {
                return false;
            }

            orderDetail.Quantity = quantity;
            return await _orderDetailRepository.UpdateOrderDetailAsync(orderDetail);
        }
        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsAsync()
        {
            return await _orderDetailRepository.GetOrderDetailsAsync();
        }
        public async Task<IEnumerable<OrderDetailForChef1DTO>> GetOrderDetails1Async()
        {
            return await _orderDetailRepository.GetOrderDetails1Async();
        }
        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync()
        {
            return await _orderDetailRepository.GetOrderDetailSummaryAsync();
        }
        public async Task UpdateDishesServedAsync(int orderDetailId, int? dishesServed)
        {
            await _orderDetailRepository.UpdateDishesServedAsync(orderDetailId, dishesServed);
        }
        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsByDishesServedAsync(int? dishesServed)
        {
            var orderDetails = await _orderDetailRepository.GetOrderDetailsByDishesServedAsync(dishesServed);
            return _mapper.Map<IEnumerable<OrderDetailForChefDTO>>(orderDetails);
        }

    }
}
