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
            var orderDetails = await _orderDetailRepository.GetOrderDetailsAsync();
            foreach (var orderDetail in orderDetails)
            {
                orderDetail.Quantity -= orderDetail.DishesServed;
                orderDetail.DishesServed = 0;
            }
            return orderDetails;
        }

        public async Task<IEnumerable<OrderDetailForChef1DTO>> GetOrderDetails1Async()
        {
            var orderDetails = await _orderDetailRepository.GetOrderDetails1Async();
            foreach (var orderDetail in orderDetails)
            {
                orderDetail.Quantity -= orderDetail.DishesServed;
                orderDetail.DishesServed = 0;
            }
            return orderDetails;
        }
        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync()
        {
            return await _orderDetailRepository.GetOrderDetailSummaryAsync();
        }
        public async Task UpdateDishesServedAsync(int orderDetailId, int? dishesServed)
        {
            var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(orderDetailId);
            if (orderDetail != null && dishesServed.HasValue)
            {
                orderDetail.DishesServed += dishesServed.Value;
                await _orderDetailRepository.UpdateOrderDetailAsync(orderDetail);
            }
            else
            {
                throw new InvalidOperationException("OrderDetail not found or invalid dishesServed value.");
            }
        }
        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsByDishesServedAsync(int? dishesServed)
        {
            var orderDetails = await _orderDetailRepository.GetOrderDetailsByDishesServedAsync(dishesServed);
            return _mapper.Map<IEnumerable<OrderDetailForChefDTO>>(orderDetails);
        }
        public async Task<IEnumerable<OrderDetailForStaff>> SearchByDishOrComboNameAsync(string keyword)
        {
            var orderDetails = await _orderDetailRepository.SearchByDishOrComboNameAsync(keyword);
            return _mapper.Map<IEnumerable<OrderDetailForStaff>>(orderDetails);
        }
    }
}
