using EHM_API.DTOs.TableDTO;
using EHM_API.Models;

namespace EHM_API.Services
{
    public interface ITableService
    {

		Task<IEnumerable<TableAllDTO>> GetAllTablesAsync();
	}
}
