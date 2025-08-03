using MigrationApp.DTOs.Category;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(Guid categoryId);
        Task<string> AddCategoryAsync(AddCategoryDto category);
        Task<string> UpdateCategoryAsync(UpdateCategoryDto category);
        Task<string> DeleteCategoryAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
    }
}