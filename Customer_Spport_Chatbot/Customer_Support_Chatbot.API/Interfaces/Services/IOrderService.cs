using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Order;
using Customer_Support_Chatbot.Wrappers;


namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IOrderService
    {
        public Task<ApiResponse<Order>> CreateOrderAsync(OrderDto orderDto);
        public Task<ApiResponse<IEnumerable<Order>>> GetAllOrdersAsync();
        public Task<ApiResponse<Order>> GetOrderByIdAsync(Guid id);
    }
}