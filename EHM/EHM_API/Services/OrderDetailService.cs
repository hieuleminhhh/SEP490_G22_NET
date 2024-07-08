using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsAsync()
        {
            return await _orderDetailRepository.GetOrderDetailsAsync();
        }

        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync()
        {
            return await _orderDetailRepository.GetOrderDetailSummaryAsync();
        }
        public async Task UpdateDishesServedAsync(List<int> orderDetailIds)
        {
            await _orderDetailRepository.UpdateDishesServedAsync(orderDetailIds);
        }

    }
}
