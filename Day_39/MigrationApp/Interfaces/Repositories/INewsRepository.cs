using MigrationApp.DTOs.News;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public interface INewsRepository
    {
        Task<IEnumerable<News>> GetAllNewsAsync();
        Task<News> GetNewsByIdAsync(Guid newsId);
        Task<string> AddNewsAsync(AddNewsDto news);
        Task<string> UpdateNewsAsync(UpdateNewsDto news);
        Task<string> DeleteNewsAsync(Guid newsId);
        Task<string> ExportNewsToCsvAsync();
    }
}