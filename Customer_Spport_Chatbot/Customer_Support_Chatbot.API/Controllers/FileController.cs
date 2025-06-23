using Customer_Support_Chatbot.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Support_Chatbot.Controllers
{
    [ApiController]
    [Route("api/v1/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] Guid ticketId)
        {
            var result = await _fileService.UploadAsync(file, ticketId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> Get(string filename)
        {
            var stream = await _fileService.DownloadAsync(filename);
            if (stream == null) return NotFound();

            return File(stream, "application/octet-stream", filename);
        }
    }
}
