using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDTO.Manager;
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
        return await _context.Orders.Include(o => o.Account)
                                    .Include(o => o.Address).
                                     ToListAsync();
    }


    public async Task<Order> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Account)
            .Include(o => o.Address)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Combo)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Dish)
            .ThenInclude(d => d.Discount)
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
								.Include(o => o.Address)
								.Include(o => o.OrderDetails)
								.ThenInclude(od => od.Combo)
								.Include(o => o.OrderDetails)
								.ThenInclude(od => od.Dish)
						    	.ThenInclude(d => d.Discount) 
								.Where(o => o.GuestPhone == guestPhone)
						         .OrderByDescending(o => o.OrderDate)
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
    public async Task<PagedResult<OrderDTO>> GetOrderAsync(string search, int page, int pageSize)
    {
        var query = _context.Orders.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(d => d.GuestPhone.ToLower().Contains(search));
        }

        var totalDishes = await query.CountAsync();

        var order = await query
            .Include(a => a.Address)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderDTOs = order.Select(o => new OrderDTO
        {
            OrderId = o.OrderId,
            OrderDate = (DateTime)o.OrderDate, 
            Status = (int)o.Status,
            RecevingOrder = o.RecevingOrder,
            AccountId = o.AccountId,
            // TableId = o.TableId,  
            InvoiceId = o.InvoiceId,
            TotalAmount = o.TotalAmount,
            GuestPhone = o.GuestPhone,
            Deposits = o.Deposits,
            AddressId = o.AddressId.HasValue ? o.AddressId.Value : 0,
            GuestAddress = o.Address?.GuestAddress,
            ConsigneeName = o.Address?.ConsigneeName
        }).ToList();

        return new PagedResult<OrderDTO>(orderDTOs, totalDishes, page, pageSize);
    }
    public async Task<Order> UpdateOrderStatusAsync(int orderId, int status)
    {
        var od = await _context.Orders.FindAsync(orderId);
        if (od == null)
        {
            return null;
        }

        od.Status = status;
        _context.Orders.Update(od);
        await _context.SaveChangesAsync();

        return od;
    }

	public async Task<IEnumerable<Order>> GetOrdersWithTablesAsync()
	{
		return await _context.Orders
			.Include(o => o.OrderTables)
			.ThenInclude(ot => ot.Table)
			.Include(o => o.Address) 
			.ToListAsync();
	}

}
