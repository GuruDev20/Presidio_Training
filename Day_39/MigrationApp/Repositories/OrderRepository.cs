using Microsoft.EntityFrameworkCore;
using MigrationApp.Contexts;
using MigrationApp.DTOs.Order;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Models;

namespace MigrationApp.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddOrderAsync(AddOrderDto order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            var orderItem = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = order.UserId,
                ProductId = order.ProductId,
                OrderDate = order.OrderDate ?? DateTime.UtcNow,
                PaymentType = order.PaymentType,
                Status = order.Status,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                CustomerAddress = order.CustomerAddress
            };
            _context.Orders.Add(orderItem);
            await _context.SaveChangesAsync();
            return orderItem.OrderId.ToString();
        }

        public async Task<string> DeleteOrderAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
            }
            var orderItem = await _context.Orders.FindAsync(orderId);
            if (orderItem == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }
            _context.Orders.Remove(orderItem);
            await _context.SaveChangesAsync();
            return "Order deleted successfully.";
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var orderList = await _context.Orders.ToListAsync();
            return orderList;
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
            }
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }
            return order;
        }
    }
}