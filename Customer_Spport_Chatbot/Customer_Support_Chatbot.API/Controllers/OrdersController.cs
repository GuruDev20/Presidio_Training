using Microsoft.AspNetCore.Mvc;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Order;
using Customer_Support_Chatbot.Services;
using Customer_Support_Chatbot.Interfaces.Services;

namespace Customer_Support_Chatbot.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderDto orderDto)
        {
            var order = await _orderService.CreateOrderAsync(orderDto);
            return Ok(order);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
    }
}
