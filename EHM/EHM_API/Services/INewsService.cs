using System.Collections.Generic;
using System.Threading.Tasks;
using EHM_API.DTOs;
using EHM_API.DTOs.NewDTO;

namespace EHM_API.Services
{
	public interface INewsService
	{
		Task<IEnumerable<NewsDTO>> GetAllNewsAsync();
	}
}
