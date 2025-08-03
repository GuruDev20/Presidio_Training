using MigrationApp.DTOs.Color;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Models;

namespace MigrationApp.Services
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        public ColorService(IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
        }

        public async Task<string> AddColorAsync(AddColorDto color)
        {
            if (color == null)
            {
                throw new ArgumentNullException(nameof(color));
            }
            return await _colorRepository.AddColorAsync(color);
        }

        public async Task<string> DeleteColorAsync(Guid colorId)
        {
            if (colorId == Guid.Empty)
            {
                throw new ArgumentException("Color ID cannot be empty.", nameof(colorId));
            }
            return await _colorRepository.DeleteColorAsync(colorId);
        }

        public async Task<IEnumerable<Color>> GetAllColorsAsync()
        {
            return await _colorRepository.GetAllColorsAsync();
        }

        public async Task<Color> GetColorByIdAsync(Guid colorId)
        {
            if (colorId == Guid.Empty)
            {
                throw new ArgumentException("Color ID cannot be empty.", nameof(colorId));
            }
            return await _colorRepository.GetColorByIdAsync(colorId);
        }

        public async Task<IEnumerable<Product>> GetProductsByColorIdAsync(Guid colorId)
        {
            if (colorId == Guid.Empty)
            {
                throw new ArgumentException("Color ID cannot be empty.", nameof(colorId));
            }
            return await _colorRepository.GetProductsByColorIdAsync(colorId);
        }

        public async Task<string> UpdateColorAsync(UpdateColorDto color)
        {
            if (color == null)
            {
                throw new ArgumentNullException(nameof(color));
            }
            return await _colorRepository.UpdateColorAsync(color);
        }
    }
}