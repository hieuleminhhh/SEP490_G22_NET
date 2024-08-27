using AutoMapper;
using EHM_API.DTOs.ComboDTO.Manager;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.OrderTableDTO;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class OrderTableService : IOrderTableService
    {
        private readonly IOrderTableRepository _repository;
        private readonly IMapper _mapper;

        public OrderTableService(IOrderTableRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<CreateOrderTable> CreateOrderTableAsync(CreateOrderTable createOrderTableDTO)
        {
            var ordertable = _mapper.Map<OrderTable>(createOrderTableDTO);
            var createdOrdertable = await _repository.AddAsync(ordertable);
            return _mapper.Map<CreateOrderTable>(createdOrdertable);
        }
    }
}
