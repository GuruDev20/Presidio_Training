using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task5_Divisible7
    {
        public static void Run()
        {
            int count = CountDivisibleBy7(10);
            Console.WriteLine($"Count of numbers divisible by 7: {count}");
        }

        public static int CountDivisibleBy7(int totalNumbers)
        {
            int count = 0;
            for (int i = 1; i <= totalNumbers; i++)
            {
                int num = GetNumber($"Enter number {i}:");
                if (num % 7 == 0)
                    count++;
            }
            return count;
        }

        public static int GetNumber(string prompt)
        {
            Console.WriteLine(prompt);
            return int.Parse(Console.ReadLine());
        }
    }
}
