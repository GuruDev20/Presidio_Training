using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Order;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        public Task<Order> CreateOrderAsync(OrderDto orderDto, string razorpayOrderId);
        public Task<IEnumerable<Order>> GetAllOrdersAsync();
        public Task<Order> GetOrderByIdAsync(Guid id);

    }
}