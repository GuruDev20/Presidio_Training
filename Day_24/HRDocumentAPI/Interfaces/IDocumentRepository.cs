using HRDocumentAPI.Models;

namespace HRDocumentAPI.Interfaces
{
    public interface IDocumentRepository
    {
        public Task<Document> AddAsync(Document doc);
        public Task<IEnumerable<Document>> GetAllAsync();
    }
}