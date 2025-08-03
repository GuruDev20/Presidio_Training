using System.Text;
using Microsoft.EntityFrameworkCore;
using MigrationApp.Contexts;
using MigrationApp.DTOs.News;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Models;

namespace MigrationApp.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _context;
        public NewsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddNewsAsync(AddNewsDto news)
        {
            if (news == null)
            {
                throw new ArgumentNullException(nameof(news));
            }
            var newsItem = new News
            {
                NewsId = Guid.NewGuid(),
                Title = news.Title,
                ShortDescription = news.ShortDescription,
                Image = news.Image,
                Content = news.Content,
                CreatedDate = DateTime.UtcNow,
                Status = news.Status
            };
            _context.News.Add(newsItem);
            await _context.SaveChangesAsync();
            return newsItem.NewsId.ToString();
        }

        public async Task<string> DeleteNewsAsync(Guid newsId)
        {
            if (newsId == Guid.Empty)
            {
                throw new ArgumentException("News ID cannot be empty.", nameof(newsId));
            }
            var newsItem = await _context.News.FindAsync(newsId);
            if (newsItem == null)
            {
                throw new KeyNotFoundException("News not found.");
            }
            _context.News.Remove(newsItem);
            await _context.SaveChangesAsync();
            return "News deleted successfully.";
        }

        public async Task<string> ExportNewsToCsvAsync()
        {
            var newsList = await _context.News.ToListAsync();
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("NewsId,Title,ShortDescription,Image,Content,CreatedDate,Status");
            foreach (var news in newsList)
            {
                csvBuilder.AppendLine($"{news.NewsId},{news.Title},{news.ShortDescription},{news.Image},{news.Content},{news.CreatedDate},{news.Status}");
            }
            string filePath = "/Users/dev/Desktop/Day_39/MigrationApp/news_export.csv";
            await File.WriteAllTextAsync(filePath, csvBuilder.ToString());
            return filePath;
        }

        public async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            var newsList = await _context.News.ToListAsync();
            return newsList;
        }

        public async Task<News> GetNewsByIdAsync(Guid newsId)
        {
            if (newsId == Guid.Empty)
            {
                throw new ArgumentException("News ID cannot be empty.", nameof(newsId));
            }
            var newsItem = await _context.News.FindAsync(newsId);
            if (newsItem == null)
            {
                throw new KeyNotFoundException("News not found.");
            }
            return newsItem;
        }

        public async Task<string> UpdateNewsAsync(UpdateNewsDto news)
        {
            if (news == null)
            {
                throw new ArgumentNullException(nameof(news));
            }
            var newsItem = await _context.News.FindAsync(news.NewsId);
            if (newsItem == null)
            {
                throw new KeyNotFoundException("News not found.");
            }
            newsItem.Title = news.Title!;
            newsItem.ShortDescription = news.ShortDescription!;
            newsItem.Image = news.Image!;
            newsItem.Content = news.Content!;
            newsItem.Status = news.Status;
            _context.News.Update(newsItem);
            await _context.SaveChangesAsync();
            return "News updated successfully.";
        }
    }
}