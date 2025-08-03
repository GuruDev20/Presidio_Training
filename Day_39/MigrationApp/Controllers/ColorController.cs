using Microsoft.AspNetCore.Mvc;
using MigrationApp.DTOs.Color;
using MigrationApp.Interfaces.Services;

namespace MigrationApp.Controllers
{
    [ApiController]
    [Route("api/color")]
    public class ColorControler : ControllerBase
    {
        private readonly IColorService _colorService;
        public ColorControler(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet("colors")]
        public async Task<IActionResult> GetColorsAsync()
        {
            var colors = await _colorService.GetAllColorsAsync();
            if (colors != null && colors.Any())
            {
                return Ok(colors);
            }
            return NotFound("No colors found.");
        }

        [HttpGet("colors/{id}")]
        public async Task<IActionResult> GetColorByIdAsync(Guid id)
        {
            var color = await _colorService.GetColorByIdAsync(id);
            if (color != null)
            {
                return Ok(color);
            }
            return NotFound($"Color with ID {id} not found.");
        }

        [HttpPost("colors")]
        public async Task<IActionResult> CreateColorAsync([FromBody] AddColorDto createColorDto)
        {
            if (createColorDto == null)
            {
                return BadRequest("Invalid color data.");
            }
            var result = await _colorService.AddColorAsync(createColorDto);
            if (result != null)
            {
                return Created("Color created successfully.", result);
            }
            return BadRequest("Failed to create color.");
        }

        [HttpPut("colors/{id}")]
        public async Task<IActionResult> UpdateColorAsync([FromBody] UpdateColorDto updateColorDto)
        {
            if (updateColorDto == null)
            {
                return BadRequest("Invalid color data or ID.");
            }
            var result = await _colorService.UpdateColorAsync(updateColorDto);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound($"Color with ID {updateColorDto.ColorId} not found.");
        }

        [HttpDelete("colors/{id}")]
        public async Task<IActionResult> DeleteColorAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid color ID.");
            }
            var result = await _colorService.DeleteColorAsync(id);
            if (result != null)
            {
                return NoContent();
            }
            return NotFound($"Color with ID {id} not found.");
        }

        // [HttpGet("colors/search")]
        // public async Task<IActionResult> SearchColorsAsync(Guid id)
        // {
        //     if (id == Guid.Empty)
        //     {
        //         return BadRequest("Invalid color ID.");
        //     }
        //     var colors = await _colorService.GetProductsByColorIdAsync(id);
        //     if (colors != null && colors.Any())
        //     {
        //         return Ok(colors);
        //     }
        //     return NotFound("No colors found matching the search criteria.");
        // }
    }
}