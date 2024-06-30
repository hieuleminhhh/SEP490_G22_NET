using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.DTOs.TableDTO;
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
    public async Task<PagedResult<OrderDTO>> GetOrderAsync(string search, DateTime? dateFrom, DateTime? dateTo, int status, int page, int pageSize, string filterByDate, int type)
    {
        var query = _context.Orders.AsQueryable();

        // Filter by search keyword (GuestPhone)
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(d => d.GuestPhone.ToLower().Contains(search));
        }

        // Filter by DateFrom and DateTo based on filterBy parameter
        if (dateFrom != null || dateTo != null)
        {
            if (filterByDate == "Đặt hàng")
            {
                if (dateFrom != null && dateTo != null)
                {
                    query = query.Where(d => d.OrderDate >= dateFrom && d.OrderDate <= dateTo.Value.AddDays(1).AddTicks(-1));
                }
                else if (dateFrom != null)
                {
                    query = query.Where(d => d.OrderDate >= dateFrom && d.OrderDate <= dateFrom.Value.AddDays(1).AddTicks(-1));
                }
                else if (dateTo != null)
                {
                    query = query.Where(d => d.OrderDate <= dateTo.Value.AddDays(1).AddTicks(-1));
                }
            }
            else if (filterByDate == "Giao hàng")
            {
                if (dateFrom != null && dateTo != null)
                {
                    query = query.Where(d => d.RecevingOrder >= dateFrom && d.RecevingOrder <= dateTo.Value.AddDays(1).AddTicks(-1));
                }
                else if (dateFrom != null)
                {
                    query = query.Where(d => d.RecevingOrder >= dateFrom && d.RecevingOrder <= dateFrom.Value.AddDays(1).AddTicks(-1));
                }
                else if (dateTo != null)
                {
                    query = query.Where(d => d.RecevingOrder <= dateTo.Value.AddDays(1).AddTicks(-1));
                }
            }
        }

        // Filter by Status
        if (status != 0) // Assuming 0 means no filter by status
        {
            query = query.Where(d => (int)d.Status == status);
        }

        // Filter by Type
        if (type != 0)
        {
            query = query.Where(d => d.Type == type);
        }

        var totalOrders = await query.CountAsync();

        var orders = await query
            .Include(a => a.Address)
            .Include(o => o.OrderTables).ThenInclude(ot => ot.Table)
            .OrderByDescending(o => filterByDate == "Đặt hàng" ? o.OrderDate : o.RecevingOrder) // Order by selected filter in descending order
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderDTOs = orders.Select(o => new OrderDTO
        {
            OrderId = o.OrderId,
            OrderDate = (DateTime)o.OrderDate,
            Status = (int)o.Status,
            RecevingOrder = o.RecevingOrder,
            AccountId = o.AccountId,
            TableIds = o.OrderTables.Select(tb => new TableAllDTO
            {
                TableId = tb.TableId,
                Capacity = tb.Table != null ? tb.Table.Capacity : default(int),
                Floor = tb.Table != null ? tb.Table.Floor : default(int),
                Status = tb.Table != null ? tb.Table.Status : default(int),
            }).ToList(),
            InvoiceId = o.InvoiceId,
            TotalAmount = o.TotalAmount,
            GuestPhone = o.GuestPhone,
            Deposits = o.Deposits,
            AddressId = o.AddressId ?? 0,
            GuestAddress = o.Address?.GuestAddress,
            ConsigneeName = o.Address?.ConsigneeName,
            Note = o.Note,
            Type = o.Type
        }).ToList();

        return new PagedResult<OrderDTO>(orderDTOs, totalOrders, page, pageSize);
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
