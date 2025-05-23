using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidPrinciples.Interfaces;
using SolidPrinciples.Models;

namespace SolidPrinciples.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger _logger;

        public OrderService(IOrderRepository orderRepository, ILogger logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }
        public void PlaceOrder(Order order)
        {
            _orderRepository.Save(order);
            _logger.Log("Order Placed");
        }
    }
}
