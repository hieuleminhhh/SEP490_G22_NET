using System.Collections.Generic;
using System.Threading.Tasks;
using EHM_API.Models;

namespace EHM_API.Repositories
{
	public interface INewsRepository
	{
		Task<IEnumerable<News>> GetAllNewsAsync();
	}
}
