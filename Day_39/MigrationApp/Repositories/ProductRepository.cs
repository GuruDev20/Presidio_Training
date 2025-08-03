using Microsoft.EntityFrameworkCore;
using MigrationApp.Contexts;
using MigrationApp.DTOs.Product;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Models;
using MigrationApp.Wrappers;

namespace MigrationApp.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> AddProductAsync(AddProductDto productDto)
        {
            if (productDto == null)
            {
                throw new ArgumentNullException(nameof(productDto), "Product data cannot be null.");
            }

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = productDto.ProductName,
                Image = productDto.Image,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId,
                ColorId = productDto.ColorId,
                // ModelId = productDto.ModelId,
                IsNew = productDto.IsNew,
                SellStartDate = productDto.SellStartDate,
                SellEndDate = productDto.SellEndDate
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Ok("Product added successfully.");
        }

        public async Task<ApiResponse<string>> DeleteProductAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
            }
            
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return ApiResponse<string>.Fail("Product not found.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Ok("Product deleted successfully.");
        }

        public async Task<ApiResponse<List<ProductResponseDto>>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Select(p => new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Image = p.Image,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    ColorId = p.ColorId,
                    // ModelId = p.ModelId,
                    IsNew = p.IsNew,
                    SellStartDate = p.SellStartDate,
                    SellEndDate = p.SellEndDate
                }).ToListAsync();

            return new ApiResponse<List<ProductResponseDto>>
            {
                Data = products,
                Message = "Products retrieved successfully",
                Success = true
            };
        }

        public async Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
            }

            var product = await _context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Image = p.Image,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    ColorId = p.ColorId,
                    // ModelId = p.ModelId,
                    IsNew = p.IsNew,
                    SellStartDate = p.SellStartDate,
                    SellEndDate = p.SellEndDate
                }).FirstOrDefaultAsync();

            if (product == null)
            {
                return ApiResponse<ProductResponseDto>.Fail("Product not found.");
            }

            return new ApiResponse<ProductResponseDto>
            {
                Data = product,
                Message = "Product retrieved successfully",
                Success = true
            };
        }

        public async Task<ApiResponse<string>> UpdateProductAsync(UpdateProductDto productDto)
        {
            if (productDto == null)
            {
                throw new ArgumentNullException(nameof(productDto), "Product data cannot be null.");
            }
            var product = _context.Products.Find(productDto.ProductId);
            if (product == null)
            {
                return ApiResponse<string>.Fail("Product not found.");
            }
            product.ProductName = productDto.ProductName!;
            product.Image = productDto.Image!;
            product.Price = productDto.Price;
            product.CategoryId = productDto.CategoryId;
            product.ColorId = productDto.ColorId;
            // product.ModelId = productDto.ModelId;
            product.IsNew = productDto.IsNew;
            product.SellStartDate = productDto.SellStartDate;
            product.SellEndDate = productDto.SellEndDate;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.Ok("Product updated successfully.");
        }
    }
}