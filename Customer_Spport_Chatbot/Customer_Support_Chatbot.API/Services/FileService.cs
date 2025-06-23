using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.DTOs.File;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
{
    public class FileService : IFileService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public FileService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<FileStream?> DownloadAsync(string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", fileName);
            if (!File.Exists(filePath))
            {
                return null;
            }
            return await Task.Run(()=>new FileStream(filePath,FileMode.Open, FileAccess.Read));
        }

        public async Task<ApiResponse<FileUploadResultDto>> UploadAsync(IFormFile file, Guid ticketId)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var attachment = new FileAttachment
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                TicketId = ticketId,
                UploadedAt = DateTime.UtcNow
            };
            _context.FileAttachments.Add(attachment);
            await _context.SaveChangesAsync();
            var fileUrl = $"/uploads/{fileName}";
            return ApiResponse<FileUploadResultDto>.Ok("File uploaded successfully",new FileUploadResultDto
            {
                FileName = fileName,
                Url = fileUrl,
                TicketId = ticketId
            });
        }
    }
}