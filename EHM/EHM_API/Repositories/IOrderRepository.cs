using EHM_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> GetByIdAsync(int id);
    Task<Order> AddAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Order>> SearchAsync(string guestPhone);
}
