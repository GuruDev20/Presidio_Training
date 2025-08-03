using MigrationApp.DTOs.Category;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Models;

namespace MigrationApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<string> AddCategoryAsync(AddCategoryDto category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            return await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task<string> DeleteCategoryAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            return await _categoryRepository.DeleteCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            return await _categoryRepository.GetCategoryByIdAsync(categoryId);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            return await _categoryRepository.GetProductsByCategoryIdAsync(categoryId);
        }

        public async Task<string> UpdateCategoryAsync(UpdateCategoryDto category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            return await _categoryRepository.UpdateCategoryAsync(category);
        }
    }
}