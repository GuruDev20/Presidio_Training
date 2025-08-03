using MigrationApp.DTOs.Product;
using MigrationApp.Wrappers;

namespace MigrationApp.Interfaces.Services
{
    public interface IProductService
    {
        Task<ApiResponse<string>> AddProductAsync(AddProductDto productDto);
        Task<ApiResponse<string>> DeleteProductAsync(Guid productId);
        Task<ApiResponse<string>> UpdateProductAsync(Guid productId, UpdateProductDto productDto);
        Task<ApiResponse<List<ProductResponseDto>>> GetAllProductsAsync();
        Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(Guid productId);
    }
}