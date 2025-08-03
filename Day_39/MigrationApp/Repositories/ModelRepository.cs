using Microsoft.EntityFrameworkCore;
using MigrationApp.Contexts;
using MigrationApp.DTOs.Model;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Models;

namespace MigrationApp.Repositories
{
    // public class ModelRepository : IModelRepository
    // {
    //     private readonly AppDbContext _context;
    //     public ModelRepository(AppDbContext context)
    //     {
    //         _context = context;
    //     }

    //     public async Task<string> AddModelAsync(AddModelDto modelDto)
    //     {
    //         if (modelDto == null)
    //         {
    //             throw new ArgumentNullException(nameof(modelDto));
    //         }
    //         var model = new Model
    //         {
    //             ModelId = Guid.NewGuid(),
    //             ModelName = modelDto.ModelName
    //         };
    //         _context.Models.Add(model);
    //         await _context.SaveChangesAsync();
    //         return model.ModelId.ToString();
    //     }

    //     public async Task<string> DeleteModelAsync(Guid modelId)
    //     {
    //         if (modelId == Guid.Empty)
    //         {
    //             throw new ArgumentException("Model ID cannot be empty.", nameof(modelId));
    //         }
    //         var model = await _context.Models.FindAsync(modelId);
    //         if (model == null)
    //         {
    //             throw new KeyNotFoundException("Model not found.");
    //         }
    //         _context.Models.Remove(model);
    //         await _context.SaveChangesAsync();
    //         return "Model deleted successfully.";
    //     }

    //     public async Task<IEnumerable<Model>> GetAllModelsAsync()
    //     {
    //         var models = await _context.Models.ToListAsync();
    //         return models;
    //     }

    //     public async Task<Model> GetModelByIdAsync(Guid modelId)
    //     {
    //         if (modelId == Guid.Empty)
    //         {
    //             throw new ArgumentException("Model ID cannot be empty.", nameof(modelId));
    //         }
    //         var model = await _context.Models.FindAsync(modelId);
    //         if (model == null)
    //         {
    //             throw new KeyNotFoundException("Model not found.");
    //         }
    //         return model;
    //     }

    //     public async Task<IEnumerable<Product>> GetProductsByModelIdAsync(Guid modelId)
    //     {
    //         if (modelId == Guid.Empty)
    //         {
    //             throw new ArgumentException("Model ID cannot be empty.", nameof(modelId));
    //         }
    //         var products = await _context.Products
    //             .Where(p => p.ModelId == modelId)
    //             .ToListAsync();
    //         return products;
    //     }

    //     public async Task<string> UpdateModelAsync(UpdateModelDto modelDto)
    //     {
    //         if (modelDto == null)
    //         {
    //             throw new ArgumentNullException(nameof(modelDto));
    //         }
    //         var model = await _context.Models.FindAsync(modelDto.ModelId);
    //         if (model == null)
    //         {
    //             throw new KeyNotFoundException("Model not found.");
    //         }
    //         model.ModelName = modelDto.ModelName;
    //         _context.Models.Update(model);
    //         await _context.SaveChangesAsync();
    //         return "Model updated successfully.";
    //     }
    // }
}