using MigrationApp.DTOs.Model;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Models;

namespace MigrationApp.Services
{
    // public class ModelService : IModelService
    // {
    //     private readonly IModelRepository _modelRepository;
    //     public ModelService(IModelRepository modelRepository)
    //     {
    //         _modelRepository = modelRepository;
    //     }

    //     public async Task<string> AddModelAsync(AddModelDto modelDto)
    //     {
    //         if (modelDto == null)
    //         {
    //             throw new ArgumentNullException(nameof(modelDto));
    //         }
    //         return await _modelRepository.AddModelAsync(modelDto);
    //     }

    //     public async  Task<string> DeleteModelAsync(Guid modelId)
    //     {
    //         if (modelId == Guid.Empty)
    //         {
    //             throw new ArgumentException("Model ID cannot be empty.", nameof(modelId));
    //         }
    //         return await _modelRepository.DeleteModelAsync(modelId);
    //     }

    //     public async Task<IEnumerable<Model>> GetAllModelsAsync()
    //     {
    //         return await _modelRepository.GetAllModelsAsync();
    //     }

    //     public async Task<Model> GetModelByIdAsync(Guid modelId)
    //     {
    //         if (modelId == Guid.Empty)
    //         {
    //             throw new ArgumentException("Model ID cannot be empty.", nameof(modelId));
    //         }
    //         return await _modelRepository.GetModelByIdAsync(modelId);
    //     }

    //     public async Task<IEnumerable<Product>> GetProductsByModelIdAsync(Guid modelId)
    //     {
    //         if (modelId == Guid.Empty)
    //         {
    //             throw new ArgumentException("Model ID cannot be empty.", nameof(modelId));
    //         }
    //         return await _modelRepository.GetProductsByModelIdAsync(modelId);
    //     }

    //     public async Task<string> UpdateModelAsync(UpdateModelDto modelDto)
    //     {
    //         if (modelDto == null)
    //         {
    //             throw new ArgumentNullException(nameof(modelDto));
    //         }
    //         return await _modelRepository.UpdateModelAsync(modelDto);
    //     }
    // }
}