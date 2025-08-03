using Microsoft.EntityFrameworkCore;
using MigrationApp.Contexts;
using MigrationApp.DTOs.Category;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddCategoryAsync(AddCategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                throw new ArgumentNullException(nameof(categoryDto));
            }
            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = categoryDto.Name,
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.CategoryId.ToString();
        }

        public async Task<string> DeleteCategoryAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return "Category deleted successfully.";
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }
            return category;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
            return products;
        }

        public async Task<string> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            if (updateCategoryDto == null)
            {
                throw new ArgumentNullException(nameof(updateCategoryDto));
            }
            var category = await _context.Categories.FindAsync(updateCategoryDto.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }
            category.Name = updateCategoryDto.Name;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return "Category updated successfully.";
        }
    }
}