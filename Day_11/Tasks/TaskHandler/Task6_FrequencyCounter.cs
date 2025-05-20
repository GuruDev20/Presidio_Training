using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task6_FrequencyCounter
    {
        public static void Run()
        {
            int[] numbers = ReadArrayFromUser("Enter the number of elements for frequency count:");
            CountFrequencies(numbers);
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

        public static void CountFrequencies(int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
            {
                Console.WriteLine("The array is empty. No frequencies to count.");
                return;
            }
            Dictionary<int, int> frequencyMap = new Dictionary<int, int>();
            foreach(int num in numbers)
            {
                if (frequencyMap.ContainsKey(num))
                {
                    frequencyMap[num]++;
                }
                else
                {
                    frequencyMap[num] = 1;
                }
            }
            Console.WriteLine("\n Frequency of each element:");
            foreach(var item in frequencyMap){
                Console.WriteLine($"{item.Key} occurs {item.Value} times");
            }
        }
    }
}
