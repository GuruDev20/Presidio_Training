using MigrationApp.DTOs.Color;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Services
{
    public interface IColorService
    {
        Task<IEnumerable<Color>> GetAllColorsAsync();
        Task<Color> GetColorByIdAsync(Guid colorId);
        Task<string> AddColorAsync(AddColorDto color);
        Task<string> UpdateColorAsync(UpdateColorDto color);
        Task<string> DeleteColorAsync(Guid colorId);
        Task<IEnumerable<Product>> GetProductsByColorIdAsync(Guid colorId);
    }
}