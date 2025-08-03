using MigrationApp.DTOs.Order;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<string> AddOrderAsync(AddOrderDto order);
        Task<string> DeleteOrderAsync(Guid orderId);
    }
}