using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OrderRepository : IOrderRepository
{
    private readonly EHMDBContext _context;

    public OrderRepository(EHMDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders.Include(o => o.Account).ToListAsync();
    }


    public async Task<Order> GetByIdAsync(int id)
    {
        return await _context.Orders
                             .Include(o => o.Account)
                             .FirstOrDefaultAsync(o => o.OrderId == id);
    }


    public async Task<Order> AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }
    public async Task<Account> GetAccountByUsernameAsync(string username)
    {
        return await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
    }

    public async Task<IEnumerable<Order>> SearchAsync(string guestPhone)
    {
        return await _context.Orders
                             .Include(o => o.Account)
                             .Where(o => o.GuestPhone == guestPhone)
                             .ToListAsync();
    }


    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Entry(order).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return false;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }


}
