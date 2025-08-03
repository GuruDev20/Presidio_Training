using MigrationApp.DTOs.Cart;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> AddItemToCartAsync(AddToCartDto addToCartDto);
        Task<Cart> RemoveItemFromCartAsync(RemoveFromCartDto removeFromCartDto);
        Task<IEnumerable<Cart>> GetCartItemsAsync(Guid userId);
        Task<string> ClearCartAsync(Guid userId);
        Task<Cart> UpdateItemQuantityAsync(UpdateCartDto updateCartDto);
        Task<string> CheckoutAsync(Guid userId);
    }
}