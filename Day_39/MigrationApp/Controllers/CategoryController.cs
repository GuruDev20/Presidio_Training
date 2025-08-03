using Microsoft.AspNetCore.Mvc;
using MigrationApp.DTOs.Category;
using MigrationApp.Interfaces.Services;

namespace MigrationApp.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories != null && categories.Any())
            {
                return Ok(categories);
            }
            return NotFound("No categories found.");
        }

        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category != null)
            {
                return Ok(category);
            }
            return NotFound($"Category with ID {id} not found.");
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] AddCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null)
            {
                return BadRequest("Invalid category data.");
            }
            var result = await _categoryService.AddCategoryAsync(createCategoryDto);
            if (result != null)
            {
                return Created("Category created successfully.", result);
            }
            return BadRequest("Failed to create category.");
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (updateCategoryDto == null)
            {
                return BadRequest("Invalid category data or ID.");
            }
            var result = await _categoryService.UpdateCategoryAsync(updateCategoryDto);
            if (result != null)
            {
                return NoContent();
            }
            return NotFound($"Category with ID {updateCategoryDto.CategoryId} not found.");
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid category ID.");
            }
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result != null)
            {
                return NoContent();
            }
            return NotFound($"Category with ID {id} not found.");
        }

        // [HttpGet("categories/search")]
        // public async Task<IActionResult> SearchCategoriesAsync(Guid id)
        // {
        //     if (id == Guid.Empty)
        //     {
        //         return BadRequest("Invalid category ID.");
        //     }
        //     var categories = await _categoryService.GetProductsByCategoryIdAsync(id);
        //     if (categories != null && categories.Any())
        //     {
        //         return Ok(categories);
        //     }
        //     return NotFound($"No categories found matching '{id}'.");
        // }


    }
}