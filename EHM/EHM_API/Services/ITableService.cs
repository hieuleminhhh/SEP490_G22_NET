using System.Collections.Generic;
using System.Threading.Tasks;
using EHM_API.DTOs.TableDTO;

namespace EHM_API.Services
{
	public interface ITableService
	{
		Task<IEnumerable<TableAllDTO>> GetAllTablesAsync();

		Task<IEnumerable<FindTableDTO>> GetAvailableTablesForGuestsAsync(int guestNumber);
	}
}
