using SolidPrinciples.Interfaces;
using SolidPrinciples.Repositories;
using SolidPrinciples.Services;
using SolidPrinciples.Utilities;
using SolidPrinciples.Models;

namespace SolidPrinciples.DemonStration
{
    public class DIP_Demo
    {
        public static void Run()
        {
            IOrderRepository repository = new OrderRepository();
            ILogger logger = new FileLogger();
            OrderService service=new OrderService(repository,logger);
            service.PlaceOrder(new Order { Id = 1, ProductName = "Laptop" });
        }
    }
}
