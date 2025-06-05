using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }
        try
        {
            await _fileService.SaveFilesAsync(file);
            return Ok("File uploaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("File name cannot be null or empty.");
        }

        try
        {
            var fileContent = await _fileService.GetFileAsync(fileName);
            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound("File not found.");
            }

            return File(fileContent, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}