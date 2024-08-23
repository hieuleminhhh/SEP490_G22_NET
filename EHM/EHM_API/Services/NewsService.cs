using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs;
using EHM_API.DTOs.NewDTO;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
	public class NewsService : INewsService
	{
		private readonly INewsRepository _newsRepository;
		private readonly IMapper _mapper;

		public NewsService(INewsRepository newsRepository, IMapper mapper)
		{
			_newsRepository = newsRepository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<NewsDTO>> GetAllNewsAsync()
		{
			var news = await _newsRepository.GetAllNewsAsync();
			return _mapper.Map<IEnumerable<NewsDTO>>(news);
		}
	}
}
