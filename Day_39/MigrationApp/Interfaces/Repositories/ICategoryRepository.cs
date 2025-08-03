using MigrationApp.DTOs.Category;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(Guid categoryId);
        Task<string> AddCategoryAsync(AddCategoryDto categoryDto);
        Task<string> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
        Task<string> DeleteCategoryAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
    }
}