using System.Runtime.InteropServices.JavaScript;
using Design_Pattern.Patterns.AbstractFactory;
using Design_Pattern.Patterns.Adapter;
using Design_Pattern.Patterns.Factory;
using Design_Pattern.Patterns.FlyWeight;
using Design_Pattern.Patterns.Proxy;
using Design_Pattern.Patterns.Singleton;

class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\nSelect a Design Pattern:");
            Console.WriteLine("1. Singleton");
            Console.WriteLine("2. Factory");
            Console.WriteLine("3. Abstract Factory");
            Console.WriteLine("4. Adapter");
            Console.WriteLine("5. Proxy");
            Console.WriteLine("6. FlyWeight");
            Console.WriteLine("7. Exit");
            Console.Write("Enter choice: ");

            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Logger log1 = Logger.GetInstance();
                    log1.Log("This is the first log message.");
                    Logger log2 = Logger.GetInstance();
                    log2.Log("This is the second log message.");
                    Console.WriteLine($"Are log1 and log2 the same instance? {ReferenceEquals(log1, log2)}");
                    break;
                case "2":
                    Console.WriteLine("Enter Product Type (A/B): ");
                    var type=Console.ReadLine();
                    IProduct product = ProductFactory.GetProduct(type);
                    product.Display();
                    break;
                case "3":
                    Console.WriteLine("Choose OS (Mac/Windows): ");
                    var os=Console.ReadLine();
                    GUIFactory factory = os == "Mac" ? new MacFactory() : new WindowsFactory();
                    IButton button = factory.CreateButton();
                    ICheckbox checkbox=factory.CreateCheckbox();
                    button.Render();
                    checkbox.Render();
                    break;
                case "4":
                    Adaptee adaptee = new();
                    ITarget target = new Adapter(adaptee);
                    target.Request();
                    break;
                case "5":
                    ISubject proxy = new ProxySubject();
                    proxy.Operation();
                    break;
                case "6":
                    for (int i = 0; i < 3; i++)
                    {
                        IShape circle = ShapeFactory.GetShape("Circle");
                        circle.Draw(i % 2 == 0 ? "Red" : "Blue");
                    }
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        }
    }
}