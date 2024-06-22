using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly EHMDBContext _context;

        public TableRepository(EHMDBContext context)
        {
            _context = context;
        }



	}
}
