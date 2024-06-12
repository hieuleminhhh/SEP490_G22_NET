using System.Collections.Generic;
using System.Threading.Tasks;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _repository;

        public TableService(ITableRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Table>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Table> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Table> CreateAsync(Table table)
        {
            return await _repository.CreateAsync(table);
        }

        public async Task<Table> UpdateAsync(Table table)
        {
            return await _repository.UpdateAsync(table);
        }

        public async Task<bool> ChangeStatusAsync(int id, string status)
        {
            return await _repository.ChangeStatusAsync(id, status);
        }

        public async Task<IEnumerable<Table>> SearchAsync(string keyword)
        {
            return await _repository.SearchAsync(keyword);
        }
    }
}
