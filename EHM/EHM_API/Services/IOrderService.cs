using EHM_API.DTOs.OrderDTO.Guest;
using EHM_API.DTOs.OrderDTO.Manager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IOrderService
    {
        Task<OrderDTOAll> CreateOrderAsync(CreateOrderDTO createOrderDto);
        Task<IEnumerable<OrderDTOAll>> GetAllOrdersAsync();

		Task<IEnumerable<SearchPhoneOrderDTO>> GetAllOrdersToSearchAsync();
		Task<OrderDTOAll> GetOrderByIdAsync(int id);
        Task<IEnumerable<SearchPhoneOrderDTO>> SearchOrdersAsync(string guestPhone = null);
        Task<OrderDTOAll> UpdateOrderAsync(int id, UpdateOrderDTO updateOrderDto);
        Task<bool> DeleteOrderAsync(int id);

		Task<bool> CancelOrderAsync(int orderId);

	}
}
