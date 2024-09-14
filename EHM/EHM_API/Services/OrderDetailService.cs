using AutoMapper;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.ComboDTO.Manager;
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
        private readonly IComboRepository _comboRepository;
        private readonly IDishRepository _dishRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IMapper mapper, IComboRepository comboRepository, IDishRepository dishRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
            _comboRepository = comboRepository;
            _dishRepository = dishRepository;
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
        public async Task<GetRemainingItemsResponseDTO> GetRemainingItemsAsync(List<int> comboIds, List<int> dishIds)
        {
            var today = DateTime.Now.Date;
            var combos = await _comboRepository.GetCombosByIdsAsync(comboIds);
            var dishes = await _dishRepository.GetDishesByIdsAsync(dishIds);
            var relevantOrderDetails = await _orderDetailRepository.GetRelevantOrderDetailsAsync(today);

            // Tính số lượng còn lại của từng Combo
            foreach (var combo in combos)
            {
                var totalComboOrdered = relevantOrderDetails
                    .Where(od => od.ComboId == combo.ComboId)
                    .Sum(od => od.Quantity ?? 0);

                combo.QuantityCombo -= totalComboOrdered;
                combo.QuantityCombo = combo.QuantityCombo < 0 ? 0 : combo.QuantityCombo;
            }

            // Tính số lượng còn lại của từng Dish
            foreach (var dish in dishes)
            {
                var totalDishOrdered = relevantOrderDetails
                    .Where(od => od.DishId == dish.DishId)
                    .Sum(od => od.Quantity ?? 0);

                dish.QuantityDish -= totalDishOrdered;
                dish.QuantityDish = dish.QuantityDish < 0 ? 0 : dish.QuantityDish;
            }

            // Map dữ liệu sang DTO để trả về
            var response = new GetRemainingItemsResponseDTO
            {
                Combos = combos.Select(c => new ComboRemainingDTO
                {
                    ComboId = c.ComboId,
                    Name = c.NameCombo,
                    QuantityRemaining = c.QuantityCombo
                }).ToList(),

                Dishes = dishes.Select(d => new DishRemainingDTO
                {
                    DishId = d.DishId,
                    Name = d.ItemName,
                    QuantityRemaining = d.QuantityDish
                }).ToList()
            };

            return response;
        }

    }
}
