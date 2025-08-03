using MigrationApp.DTOs.News;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Models;

namespace MigrationApp.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<string> AddNewsAsync(AddNewsDto news)
        {
            if (news == null)
            {
                throw new ArgumentNullException(nameof(news));
            }
            return await _newsRepository.AddNewsAsync(news);
        }

        public async Task<string> DeleteNewsAsync(Guid newsId)
        {
            if (newsId == Guid.Empty)
            {
                throw new ArgumentException("News ID cannot be empty.", nameof(newsId));
            }
            return await _newsRepository.DeleteNewsAsync(newsId);
        }

        public async Task<string> ExportNewsToCsvAsync()
        {
            return await _newsRepository.ExportNewsToCsvAsync();
        }

        public async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            return await _newsRepository.GetAllNewsAsync();
        }

        public async Task<News> GetNewsByIdAsync(Guid newsId)
        {
            if (newsId == Guid.Empty)
            {
                throw new ArgumentException("News ID cannot be empty.", nameof(newsId));
            }
            return await _newsRepository.GetNewsByIdAsync(newsId);
        }

        public async Task<string> UpdateNewsAsync(UpdateNewsDto news)
        {
            if (news == null)
            {
                throw new ArgumentNullException(nameof(news));
            }
            return await _newsRepository.UpdateNewsAsync(news);
        }
    }
}