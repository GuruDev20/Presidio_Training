using MigrationApp.Contexts;
using MigrationApp.DTOs.Cart;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Models;

namespace MigrationApp.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> AddItemToCartAsync(AddToCartDto addToCartDto)
        {
            if (addToCartDto == null)
            {
                throw new ArgumentNullException(nameof(addToCartDto));
            }
            var cartItem = new Cart
            {
                CartId = Guid.NewGuid(),
                UserId = addToCartDto.UserId,
                ProductId = addToCartDto.ProductId,
                Quantity = addToCartDto.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Carts.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<string> CheckoutAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList();
            if (!cartItems.Any())
            {
                return "No items in cart to checkout.";
            }
            // Here you would typically process the order, e.g., create an order record, charge payment, etc.
            // For simplicity, we will just clear the cart.
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return "Checkout successful. Cart cleared.";
        }

        public async Task<string> ClearCartAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList();
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return "Cart cleared successfully.";
        }

        public async Task<IEnumerable<Cart>> GetCartItemsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList();
            return await Task.FromResult(cartItems);
        }

        public async Task<Cart> RemoveItemFromCartAsync(RemoveFromCartDto removeFromCartDto)
        {
            if (removeFromCartDto == null)
            {
                throw new ArgumentNullException(nameof(removeFromCartDto));
            }
            var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == removeFromCartDto.UserId && c.ProductId == removeFromCartDto.ProductId);
            if (cartItem == null)
            {
                throw new KeyNotFoundException("Item not found in cart.");
            }
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<Cart> UpdateItemQuantityAsync(UpdateCartDto updateCartDto)
        {
            if (updateCartDto == null)
            {
                throw new ArgumentNullException(nameof(updateCartDto));
            }
            var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == updateCartDto.UserId && c.ProductId == updateCartDto.ProductId);
            if (cartItem == null)
            {
                throw new KeyNotFoundException("Item not found in cart.");
            }
            cartItem.Quantity = updateCartDto.Quantity;
            _context.Carts.Update(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }
    }
}