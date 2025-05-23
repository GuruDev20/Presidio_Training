using SolidPrinciples.DemonStration;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("SOLID Principles Demonstration");
            Console.WriteLine("1. Single Responsibility Principle");
            Console.WriteLine("2. Open/Closed Principle");
            Console.WriteLine("3. Liskov Substitution Principle");
            Console.WriteLine("4. Interface Segregation Principle");
            Console.WriteLine("5. Dependency Inversion Principle");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            string? choice= Console.ReadLine();
            switch (choice)
            {
                case "1":
                    SRP_Demo.Run();
                    break;
                case "2":
                    OCP_Demo.Run();
                    break;
                case "3":
                    LSP_Demo.Run();
                    break;
                case "4":
                    ISP_Demo.Run();
                    break;
                case "5":
                    DIP_Demo.Run();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}