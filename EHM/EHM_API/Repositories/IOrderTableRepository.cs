using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IOrderTableRepository
    {
        Task<OrderTable> AddAsync(OrderTable orderTable);
    }
}
