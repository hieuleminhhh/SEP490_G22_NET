using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EHM_API.Models;

namespace EHM_API.Repositories
{
	public class NewsRepository : INewsRepository
	{
		private readonly EHMDBContext _context;

		public NewsRepository(EHMDBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<News>> GetAllNewsAsync()
		{
			return await _context.News.ToListAsync();
		}
	}
}
