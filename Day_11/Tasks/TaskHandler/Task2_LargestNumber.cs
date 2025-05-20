using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task2_LargestNumber
    {   
        public static void PrintLargest(int num1,int num2)
        {
            if (num1 > num2)
            {
                Console.WriteLine($"{num1} is largest");
            }
            else if(num1 < num2)
            {
                Console.WriteLine($"{num2} is largest");
            }
            else
            {
                Console.WriteLine("Both are equal");
            }
        }
        public static void Run()
        {
            Console.WriteLine("Enter first number:");
            int num1=int.Parse(Console.ReadLine());
            Console.WriteLine("Enter second number:");
            int num2=int.Parse(Console.ReadLine());
            PrintLargest(num1, num2);
        }
    }
}
