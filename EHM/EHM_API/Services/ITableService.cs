using System.Collections.Generic;
using System.Threading.Tasks;
using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.TableDTO;
using EHM_API.DTOs.TableDTO.Manager;
using EHM_API.DTOs.Table_ReservationDTO;
using EHM_API.Models;

namespace EHM_API.Services
{
	public interface ITableService
	{
		Task<IEnumerable<TableAllDTO>> GetAllTablesAsync();
		Task<TableAllDTO> GetTableByIdAsync(int tableId);
		Task<CreateTableDTO> AddAsync(CreateTableDTO tabledto);
        Task<CreateTableDTO> UpdateAsync(int tableId, CreateTableDTO tabledto);
        Task<IEnumerable<FindTableDTO>> GetAvailableTablesForGuestsAsync(int guestNumber);
		Task<bool> ExistTable(int tableId);
        Task DeleteTableWithDependenciesAsync(int tableId);
        Task SetTablesFloorAsync(UpdateFloorDTO updateFloorDTO);
        IEnumerable<TableReservationAllDTO> GetTableReservationsByDate(DateTime reservationTime);
    }
}
