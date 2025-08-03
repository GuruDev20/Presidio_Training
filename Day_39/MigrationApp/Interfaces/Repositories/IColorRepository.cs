using MigrationApp.DTOs.Color;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public interface IColorRepository
    {
        Task<IEnumerable<Color>> GetAllColorsAsync();
        Task<Color> GetColorByIdAsync(Guid colorId);
        Task<string> AddColorAsync(AddColorDto addColorDto);
        Task<string> UpdateColorAsync(UpdateColorDto updateColorDto);
        Task<string> DeleteColorAsync(Guid colorId);
        Task<IEnumerable<Product>> GetProductsByColorIdAsync(Guid colorId);
    }
}