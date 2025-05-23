using SolidPrinciples.Interfaces;
using SolidPrinciples.Models;

namespace SolidPrinciples.Repositories
{
    public class OrderRepository:IOrderRepository
    {
        public void Save(Order order)
        {
            Console.WriteLine($"Order Svaed: {order.ProductName}");
        }
    }
}
