using HRDocumentAPI.DTOs;
using HRDocumentAPI.Hubs;
using HRDocumentAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HRDocumentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IHubContext<NotifyHub> _hub;

        public DocumentController(IDocumentService documentService, IHubContext<NotifyHub> hub)
        {
            _hub = hub;
            _documentService = documentService;
        }

        [HttpPost("upload")]
        [Authorize(Policy ="HROnly")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var userEmail = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User email not found in token.");
            }
            var uploadedDoc = await _documentService.UploadAsync(file, userEmail);
            return Ok(uploadedDoc);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var docs = await _documentService.GetAllDocumentsAsync();
            return Ok(docs);
        }
    }
}
