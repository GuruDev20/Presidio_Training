using MigrationApp.DTOs.Model;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public interface IModelRepository
    {
        Task<string> AddModelAsync(AddModelDto modelDto);
        Task<string> UpdateModelAsync(UpdateModelDto modelDto);
        Task<string> DeleteModelAsync(Guid modelId);
        Task<IEnumerable<Model>> GetAllModelsAsync();
        Task<Model> GetModelByIdAsync(Guid modelId);
        Task<IEnumerable<Product>> GetProductsByModelIdAsync(Guid modelId);
    }
}