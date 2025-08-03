using MigrationApp.DTOs.Product;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Wrappers;

namespace MigrationApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ApiResponse<string>> AddProductAsync(AddProductDto productDto)
        {
            if (productDto == null)
            {
                throw new ArgumentNullException(nameof(productDto), "Product data cannot be null.");
            }
            return await _productRepository.AddProductAsync(productDto);
        }

        public async Task<ApiResponse<string>> DeleteProductAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
            }
            return await _productRepository.DeleteProductAsync(productId);
        }

        public async Task<ApiResponse<List<ProductResponseDto>>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        public async Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
            }
            return await _productRepository.GetProductByIdAsync(productId);
        }

        public async Task<ApiResponse<string>> UpdateProductAsync(Guid productId, UpdateProductDto productDto)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
            }
            if (productDto == null)
            {
                throw new ArgumentNullException(nameof(productDto), "Product data cannot be null.");
            }
            return await _productRepository.UpdateProductAsync(productDto);
        }
    }
}