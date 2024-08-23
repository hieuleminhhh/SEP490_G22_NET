using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EHM_API.DTOs;
using EHM_API.Services;
using EHM_API.DTOs.NewDTO;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NewsController : ControllerBase
	{
		private readonly INewsService _newsService;

		public NewsController(INewsService newsService)
		{
			_newsService = newsService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<NewsDTO>>> GetAllNews()
		{
			var news = await _newsService.GetAllNewsAsync();
			return Ok(news);
		}
	}
}
