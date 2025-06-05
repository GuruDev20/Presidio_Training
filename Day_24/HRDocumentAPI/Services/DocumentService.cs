using HRDocumentAPI.Contexts;
using HRDocumentAPI.DTOs;
using HRDocumentAPI.Interfaces;
using HRDocumentAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using HRDocumentAPI.Hubs;

namespace HRDocumentAPI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly HRDocumentAPIContext _context;
        private readonly IHubContext<NotifyHub> _hubContext;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public DocumentService(IWebHostEnvironment webHostEnvironment, HRDocumentAPIContext context, IHubContext<NotifyHub> hubContext)
        {
            _hubContext = hubContext;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync()
        {
            var documents = await _context.Documents
                .Include(d => d.UploadedBy)
                .ToListAsync();

            return documents.Select(doc => new DocumentDto
                {
                    FileName = doc.Filname,
                    Url = doc.FilePath,
                    UploadedBy = doc.UploadedBy?.Email ?? "Unknown",
                    UploadedAt = doc.UploadedAt
                });
        }

        public async Task<DocumentDto> UploadAsync(IFormFile file, string userEmail)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is empty or not provided.");
            }
            var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            var user=await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            var documentDto = new Document
            {
                Id = Guid.NewGuid(),
                Filname = file.FileName,
                FilePath = filePath,
                FileType = file.ContentType,
                Status = "Uploaded",
                UploadedAt = DateTime.UtcNow,
                UploadedById = user.Id,
            };
            _context.Documents.Add(documentDto);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("DocumentUploaded", new { 
                user = user.FullName, 
                filename = documentDto.Filname 
            });

            return new DocumentDto
            {
                FileName = documentDto.Filname,
                Url = documentDto.FilePath,
                UploadedBy = user.Email,
                UploadedAt = documentDto.UploadedAt
            };
        }
    }
}