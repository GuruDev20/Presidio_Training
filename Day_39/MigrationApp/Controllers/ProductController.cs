using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigrationApp.DTOs.Product;
using MigrationApp.Interfaces.Services;

namespace MigrationApp.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("addProduct")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Product data cannot be null.");
            }
            var response = await _productService.AddProductAsync(productDto);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpGet("getProduct/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Product ID cannot be empty.");
            }
            var response = await _productService.GetProductByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response);
        }

        [HttpDelete("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Product ID cannot be empty.");
            }
            var response = await _productService.DeleteProductAsync(id);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response);
        }

        [HttpPut("updateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto productDto)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Product ID cannot be empty.");
            }
            if (productDto == null)
            {
                return BadRequest("Product data cannot be null.");
            }
            var response = await _productService.UpdateProductAsync(id, productDto);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
    }
}