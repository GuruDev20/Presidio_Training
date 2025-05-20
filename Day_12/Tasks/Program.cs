using System;
using Tasks.TaskHandler;

namespace Tasks
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Task no:");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Task1_InstagramPostsApp.Run();
                    break;
                case "2":
                    Task2_ProductDictionary.Run();
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}
