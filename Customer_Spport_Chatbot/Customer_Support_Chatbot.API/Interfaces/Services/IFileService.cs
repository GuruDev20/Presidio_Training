using Customer_Support_Chatbot.DTOs.File;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IFileService
    {
        public Task<ApiResponse<FileUploadResultDto>> UploadAsync(IFormFile file, Guid ticketId);
        public Task<FileStream?> DownloadAsync(string fileName);
    }
}