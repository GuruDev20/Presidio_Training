using MigrationApp.DTOs.Cart;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Models;
using MigrationApp.Wrappers;

namespace MigrationApp.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<ApiResponse<Cart>> AddItemToCartAsync(AddToCartDto addToCartDto)
        {
            if (addToCartDto == null)
            {
                return ApiResponse<Cart>.Fail("Invalid cart item.");
            }
            var cartItem = await _cartRepository.AddItemToCartAsync(addToCartDto);
            return ApiResponse<Cart>.Ok("Item added to cart successfully.",cartItem);
        }

        public async Task<ApiResponse<string>> CheckoutAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "User ID cannot be empty."
                };
            }
            return ApiResponse<string>.Ok("Checkout successful.", await _cartRepository.CheckoutAsync(userId));
        }

        public async Task<ApiResponse<string>> ClearCartAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "User ID cannot be empty."
                };
            }   
            return ApiResponse<string>.Ok("Cart cleared successfully.", await _cartRepository.ClearCartAsync(userId));
        }

        public async Task<ApiResponse<IEnumerable<Cart>>> GetCartItemsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var cartItems = await _cartRepository.GetCartItemsAsync(userId);
            return ApiResponse<IEnumerable<Cart>>.Ok("Cart items retrieved successfully.", cartItems);
        }

        public async Task<ApiResponse<Cart>> RemoveItemFromCartAsync(RemoveFromCartDto removeFromCartDto)
        {
            if (removeFromCartDto == null)
            {
                throw new ArgumentNullException(nameof(removeFromCartDto));
            }
            var cartItem = await _cartRepository.RemoveItemFromCartAsync(removeFromCartDto);
            if (cartItem == null)
            {
                return ApiResponse<Cart>.Fail("Item not found in cart.");
            }
            return ApiResponse<Cart>.Ok("Item removed from cart successfully.", cartItem);
        }

        public async Task<ApiResponse<Cart>> UpdateItemQuantityAsync(UpdateCartDto updateCartDto)
        {
            if (updateCartDto == null)
            {
                throw new ArgumentNullException(nameof(updateCartDto));
            }
            var cartItem = await _cartRepository.UpdateItemQuantityAsync(updateCartDto);
            if (cartItem == null)
            {
                return ApiResponse<Cart>.Fail("Item not found in cart.");
            }
            return ApiResponse<Cart>.Ok("Item quantity updated successfully.", cartItem);
        }
    }
}