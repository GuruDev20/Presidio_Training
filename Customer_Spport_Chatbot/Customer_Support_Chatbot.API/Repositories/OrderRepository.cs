using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Order;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private new readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(OrderDto orderDto, string razorpayOrderId)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = orderDto.CustomerName,
                Email = orderDto.Email,
                ContactNumber = orderDto.ContactNumber,
                RazorpayOrderId = razorpayOrderId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Order ID cannot be empty.", nameof(id));
            }

            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}