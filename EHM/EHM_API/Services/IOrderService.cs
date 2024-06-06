using EHM_API.DTOs.OrderDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IOrderService
    {
        Task<OrderDTOAll> CreateOrderAsync(CreateOrderDTO createOrderDto);
        Task<IEnumerable<OrderDTOAll>> GetAllOrdersAsync();
        Task<OrderDTOAll> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDTOAll>> SearchOrdersAsync(string guestPhone = null);
        Task<OrderDTOAll> UpdateOrderAsync(int id, UpdateOrderDTO updateOrderDto);
        Task<bool> DeleteOrderAsync(int id);



    }
}
