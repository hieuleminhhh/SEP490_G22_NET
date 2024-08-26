using EHM_API.DTOs.OrderDTO.Guest;
using EHM_API.DTOs.OrderTableDTO;

namespace EHM_API.Services
{
    public interface IOrderTableService
    {
        Task<CreateOrderTable> CreateOrderTableAsync(CreateOrderTable createOrderTableDTO);
    }
}
