using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task1_GreetUser
    {
        public static void Run()
        {
            Console.Write("Enter your name: ");
            string input = Console.ReadLine();
            string name = string.IsNullOrWhiteSpace(input) ? "Guest" : input;
            Console.WriteLine($"Hello, {name}! Welcome to the program.");
        }
    }
}
