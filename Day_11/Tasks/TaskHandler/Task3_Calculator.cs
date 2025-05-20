using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task3_Calculator
    {
        public static void Run()
        {
            double num1 = GetInput("Enter first number:");
            double num2 = GetInput("Enter second number:");
            string op = GetOperator();
            if(Calculate(num1,num2,op,out double result))
            {
                Console.WriteLine($"Result: {result}");
            }
        }

        public static double GetInput(string prompt)
        {
            Console.WriteLine(prompt);
            return double.Parse(Console.ReadLine());
        }
        public static string GetOperator()
        {
            Console.WriteLine("Choose operation (+, -, *, /):");
            return Console.ReadLine();
        }

        public static bool Calculate(double num1,double num2,string op,out double result)
        {
            result = 0;
            switch (op)
            {
                case "+":
                    result = num1 + num2;
                    return true;
                case "-":
                    result = num1 - num2;
                    return true;
                case "*":
                    result = num1 * num2;
                    return true;
                case "/":
                    if (num2 == 0)
                    {
                        Console.WriteLine("Cannot divide by zero!");
                        return false;
                    }
                    result = num1 / num2;
                    return true;
                default:
                    Console.WriteLine("Invalid operation.");
                    return false;
            }
        }
    }
}
