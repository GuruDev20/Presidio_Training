using MigrationApp.DTOs.Order;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Models;

namespace MigrationApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<string> AddOrderAsync(AddOrderDto order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            return await _orderRepository.AddOrderAsync(order);
        }

        public async Task<string> DeleteOrderAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
            }
            return await _orderRepository.DeleteOrderAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
            }
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }
    }
}