using Microsoft.EntityFrameworkCore;
using MigrationApp.Contexts;
using MigrationApp.DTOs.Color;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Models;

namespace MigrationApp.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly AppDbContext _context;
        public ColorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddColorAsync(AddColorDto addColorDto)
        {
            if (addColorDto == null)
            {
                throw new ArgumentNullException(nameof(addColorDto));
            }
            var color = new Color
            {
                ColorId = Guid.NewGuid(),
                ColorName = addColorDto.Name,
            };
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return color.ColorId.ToString();
        }

        public async Task<string> DeleteColorAsync(Guid colorId)
        {
            var color = await GetColorByIdAsync(colorId);
            if (color == null)
            {
                throw new KeyNotFoundException($"Color with ID {colorId} not found.");
            }
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return "Color deleted successfully.";
        }

        public async Task<IEnumerable<Color>> GetAllColorsAsync()
        {
            var colors = await _context.Colors.ToListAsync();
            return colors;
        }

        public async Task<Color> GetColorByIdAsync(Guid colorId)
        {
            if (colorId == Guid.Empty)
            {
                throw new ArgumentException("Color ID cannot be empty.", nameof(colorId));
            }
            var color = await _context.Colors.FindAsync(colorId);
            if (color == null)
            {
                throw new KeyNotFoundException("Color not found.");
            }
            return color;
        }

        public async Task<IEnumerable<Product>> GetProductsByColorIdAsync(Guid colorId)
        {
            if (colorId == Guid.Empty)
            {
                throw new ArgumentException("Color ID cannot be empty.", nameof(colorId));
            }
            var products = await _context.Products
                .Where(p => p.ColorId == colorId)
                .ToListAsync();
            return products;
        }

        public async Task<string> UpdateColorAsync(UpdateColorDto updateColorDto)
        {
            if (updateColorDto == null)
            {
                throw new ArgumentNullException(nameof(updateColorDto));
            }
            var color = await GetColorByIdAsync(updateColorDto.ColorId);
            if (color == null)
            {
                throw new KeyNotFoundException($"Color with ID {updateColorDto.ColorId} not found.");
            }
            color.ColorName = updateColorDto.Name;
            _context.Colors.Update(color);
            await _context.SaveChangesAsync();
            return "Color updated successfully.";
        }
    }
}