using MigrationApp.DTOs.Order;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Services
{
    public interface IOrderService
    {
        Task<string> AddOrderAsync(AddOrderDto order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<string> DeleteOrderAsync(Guid orderId);
    }
}