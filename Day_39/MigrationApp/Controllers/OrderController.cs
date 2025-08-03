using Microsoft.AspNetCore.Mvc;
using MigrationApp.DTOs.Order;
using MigrationApp.Interfaces.Services;

namespace MigrationApp.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrderAsync([FromBody] AddOrderDto createOrder)
        {
            if (createOrder == null)
            {
                return BadRequest("Invalid order data.");
            }

            var result = await _orderService.AddOrderAsync(createOrder);
            if (result != null)
            {
                return Created("Order created successfully.", result);
            }
            return BadRequest("Failed to create order.");
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order != null)
            {
                return Ok(order);
            }
            return NotFound($"Order with ID {id} not found.");
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            if (orders != null && orders.Any())
            {
                return Ok(orders);
            }
            return NotFound("No orders found.");
        }
    }
}