using MigrationApp.DTOs.News;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Services
{
    public interface INewsService
    {
        Task<string> AddNewsAsync(AddNewsDto news);
        Task<IEnumerable<News>> GetAllNewsAsync();
        Task<News> GetNewsByIdAsync(Guid newsId);
        Task<string> UpdateNewsAsync(UpdateNewsDto news);
        Task<string> DeleteNewsAsync(Guid newsId);
        Task<string> ExportNewsToCsvAsync(); 

    }
}