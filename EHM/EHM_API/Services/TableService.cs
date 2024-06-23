using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs.TableDTO;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _repository;
		private readonly IMapper _mapper;


		public TableService(ITableRepository repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

		public async Task<IEnumerable<TableAllDTO>> GetAllTablesAsync()
		{
			var tables = await _repository.GetAllTablesAsync();
			return _mapper.Map<IEnumerable<TableAllDTO>>(tables);
		}

	}
}
