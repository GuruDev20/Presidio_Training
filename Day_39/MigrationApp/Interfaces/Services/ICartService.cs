using MigrationApp.DTOs.Cart;
using MigrationApp.Models;
using MigrationApp.Wrappers;

namespace MigrationApp.Interfaces.Services
{
    public interface ICartService
    {
        Task<ApiResponse<Cart>> AddItemToCartAsync(AddToCartDto addToCartDto);
        Task<ApiResponse<Cart>> RemoveItemFromCartAsync(RemoveFromCartDto removeFromCartDto);
        Task<ApiResponse<Cart>> UpdateItemQuantityAsync(UpdateCartDto updateCartDto);
        Task<ApiResponse<string>> ClearCartAsync(Guid userId);
        Task<ApiResponse<IEnumerable<Cart>>> GetCartItemsAsync(Guid userId);
        Task<ApiResponse<string>> CheckoutAsync(Guid userId);
    }
}