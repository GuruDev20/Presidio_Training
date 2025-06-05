using HRDocumentAPI.DTOs;

namespace HRDocumentAPI.Interfaces
{
    public interface IDocumentService
    {
        public Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync();
        public Task<DocumentDto> UploadAsync(IFormFile file, string userEmail);
        
    }
}