using Microsoft.AspNetCore.Mvc;
using MigrationApp.DTOs.News;
using MigrationApp.Interfaces.Services;

namespace MigrationApp.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("news")]
        public async Task<IActionResult> GetNewsAsync()
        {
            var news = await _newsService.GetAllNewsAsync();
            if (news != null && news.Any())
            {
                return Ok(news);
            }
            return NotFound("No news found.");
        }

        [HttpGet("news/{id}")]
        public async Task<IActionResult> GetNewsByIdAsync(Guid id)
        {
            var newsItem = await _newsService.GetNewsByIdAsync(id);
            if (newsItem != null)
            {
                return Ok(newsItem);
            }
            return NotFound($"News item with ID {id} not found.");
        }

        [HttpPost("news")]
        public async Task<IActionResult> CreateNewsAsync([FromBody] AddNewsDto createNewsDto)
        {
            if (createNewsDto == null)
            {
                return BadRequest("Invalid news data.");
            }
            var result = await _newsService.AddNewsAsync(createNewsDto);
            if (result != null)
            {
                return Created("News created successfully.", result);
            }
            return BadRequest("Failed to create news.");
        }

        [HttpPut("news/{id}")]
        public async Task<IActionResult> UpdateNewsAsync([FromBody] UpdateNewsDto updateNewsDto)
        {
            if (updateNewsDto == null)
            {
                return BadRequest("Invalid news data or ID.");
            }
            var result = await _newsService.UpdateNewsAsync(updateNewsDto);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound($"News item with ID {updateNewsDto.NewsId} not found.");
        }

        [HttpDelete("news/{id}")]
        public async Task<IActionResult> DeleteNewsAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid news ID.");
            }
            var result = await _newsService.DeleteNewsAsync(id);
            if (result != null)
            {
                return NoContent();
            }
            return NotFound($"News item with ID {id} not found.");
        }

    }
}