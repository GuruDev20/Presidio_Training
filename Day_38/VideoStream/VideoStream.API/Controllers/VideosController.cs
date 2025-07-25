using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoStream.API.Contexts;
using VideoStream.API.DTOs;
using VideoStream.API.Models;

namespace VideoStream.API.Controlers
{
    [ApiController]
    [Route("api/videos")]
    public class VideosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly BlobContainerClient _blobContainerClient;
        public VideosController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            var blobServiceClient = new BlobServiceClient(config["AzureBlobStorage:ConnectionString"]);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(config["AzureBlobStorage:ContainerName"]);
            _blobContainerClient.CreateIfNotExists();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm]VideoUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(dto.File.FileName);
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await using (var stream = dto.File.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }
            var video = new TrainingVideo
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                BlobUrl = blobClient.Uri.ToString(),
                UploadDate = DateTime.UtcNow
            };
            _context.TrainingVideos.Add(video);
            await _context.SaveChangesAsync();
            return Ok(video);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var videos = await _context.TrainingVideos
                            .OrderByDescending(v => v.UploadDate)
                            .ToListAsync();
            return Ok(videos);
        }

        [HttpGet("{id}/stream")]
        public async Task<IActionResult> StreamVideo(Guid id)
        {
            var video = await _context.TrainingVideos.FindAsync(id);
            if (video == null)
            {
                return NotFound("Video not found.");
            }
            var stream=await new HttpClient().GetStreamAsync(video.BlobUrl);
            return File(stream, "video/mp4", video.Title + ".mp4", true);
        }

    }
}