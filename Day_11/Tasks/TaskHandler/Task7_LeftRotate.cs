using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public  class Task7_LeftRotate
    {
        public static void Run()
        {
            int[] numbers = ReadArrayFromUser("Enter the number of elements for left rotation:");
            RotateLeftByOne(numbers);
            Console.WriteLine("Array after left rotation");
            Console.WriteLine(string.Join(", ", numbers));
        }

        public static int[] ReadArrayFromUser(string prompt)
        {
            Console.WriteLine(prompt);
            int size;
            while (!int.TryParse(Console.ReadLine(), out size) || size <= 0)
            {
                Console.Write("Invalid size. Please enter a positive integer: ");
            }

            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                Console.Write($"Enter element {i + 1}: ");
                while (!int.TryParse(Console.ReadLine(), out array[i]))
                {
                    Console.Write("Invalid input. Please enter a valid number: ");
                }
            }

            return array;
        }

        public static void RotateLeftByOne(int[] array)
        {
            if (array == null || array.Length == 0)
            {
                Console.WriteLine("Array is empty. Nothing to rotate.");
                return;
            }
            if (array.Length == 1)
            {
                Console.WriteLine("Array contains only one element.");
                return;
            }
            int first = array[0];
            for(int i=1;i<array.Length;i++)
            {
                array[i-1]=array[i];
            }
            array[array.Length - 1] = first;
        }
    }
}
