using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigrationApp.DTOs.Auth;
using MigrationApp.DTOs.Cart;
using MigrationApp.Interfaces.Services;
using MigrationApp.Wrappers;

namespace MigrationApp.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCartAsync([FromBody] AddToCartDto addToCartDto)
        {
            if (addToCartDto == null)
            {
                return BadRequest("Invalid cart item.");
            }

            var result = await _cartService.AddItemToCartAsync(addToCartDto);
            if (result != null)
            {
                return Ok(result.Data);
            }
            return BadRequest("Failed to add item to cart.");
        }

        [HttpGet("get-cart-items")]
        public async Task<IActionResult> GetCartItemsAsync()
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<UserProfileDto>.Fail("Invalid token"));
            }
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            if (cartItems != null)
            {
                return Ok(cartItems.Data);
            }
            return NotFound("No items found in the cart.");
        }

        [HttpDelete("remove-from-cart/{itemId}")]
        public async Task<IActionResult> RemoveFromCartAsync(Guid itemId)
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<UserProfileDto>.Fail("Invalid token"));
            }
            if (itemId == Guid.Empty)
            {
                return BadRequest("Invalid item ID.");
            }
            var removeFromCartDto = new RemoveFromCartDto
            {
                UserId = userId,
                ProductId = itemId
            };
            var result = await _cartService.RemoveItemFromCartAsync(removeFromCartDto);
            if (result != null)
            {
                return Ok("Item removed from cart successfully.");
            }
            return NotFound("Item not found in the cart.");
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutAsync()
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<UserProfileDto>.Fail("Invalid token"));
            }
            var result = await _cartService.CheckoutAsync(userId);
            if (result != null)
            {
                return Ok("Checkout successful.");
            }
            return BadRequest("Checkout failed. Please try again.");
        }

        [HttpPost("clear-cart")]
        public async Task<IActionResult> ClearCartAsync()
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<UserProfileDto>.Fail("Invalid token"));
            }
            var result = await _cartService.ClearCartAsync(userId);
            if (result != null)
            {
                return Ok("Cart cleared successfully.");
            }
            return BadRequest("Failed to clear cart.");
        }

        [HttpPost("update-item-quantity")]
        public async Task<IActionResult> UpdateItemQuantityAsync([FromBody] UpdateCartDto updateCartDto)
        {
            if (updateCartDto == null)
            {
                return BadRequest("Invalid update request.");
            }

            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<UserProfileDto>.Fail("Invalid token"));
            }

            updateCartDto.UserId = userId;
            var result = await _cartService.UpdateItemQuantityAsync(updateCartDto);
            if (result != null)
            {
                return Ok(result.Data);
            }
            return NotFound("Item not found in the cart.");
        }

    }
}