using HRDocumentAPI.Contexts;
using HRDocumentAPI.Interfaces;
using HRDocumentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HRDocumentAPI.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly HRDocumentAPIContext _context;
        public DocumentRepository(HRDocumentAPIContext context)
        {
            _context = context;
        }

        public async Task<Document> AddAsync(Document doc)
        {
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();
            return doc;
        }
        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents.Include(d=>d.UploadedBy).ToListAsync();
        }
    }
}