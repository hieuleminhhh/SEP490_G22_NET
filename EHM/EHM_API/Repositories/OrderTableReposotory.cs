using EHM_API.Models;

namespace EHM_API.Repositories
{
    public class OrderTableReposotory : IOrderTableRepository
    {
        private readonly EHMDBContext _context;

        public OrderTableReposotory(EHMDBContext context)
        {
            _context = context;
        }
        public async Task<OrderTable> AddAsync(OrderTable orderTable)
        {
            _context.OrderTables.Add(orderTable);
            await _context.SaveChangesAsync();
            return orderTable;
        }
    }
}
