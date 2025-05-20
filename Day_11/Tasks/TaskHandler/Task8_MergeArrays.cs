using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task8_MergeArrays
    {
        public static void Run()
        {
            int[] array1 = ReadArrayFromUser("Enter elements of the first array:");
            int[] array2 = ReadArrayFromUser("Enter elements of the second array:");

            int[] mergeArray = MergeArrays(array1, array2);
            Console.WriteLine("Merged Array");
            Console.WriteLine(string.Join(", ", mergeArray));
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

        public static int[] MergeArrays(int[] array1, int[] array2)
        {
            if ((array1 == null || array1.Length == 0) && (array2 == null || array2.Length == 0))
            {
                Console.WriteLine("Both arrays are empty. Nothing to merge.");
                return Array.Empty<int>();
            }
            if (array1 == null || array1.Length == 0)
            {
                Console.WriteLine("First array is empty. Returning second array.");
                return array2.ToArray();
            }
            if (array2 == null || array2.Length == 0)
            {
                Console.WriteLine("Second array is empty. Returning first array.");
                return array1.ToArray(); 
            }
            int[] result=new int[array1.Length+array2.Length];
            for(int i = 0; i < array1.Length; i++)
            {
                result[i]=array1[i];
            }
            for(int i = 0; i < array2.Length; i++)
            {
                result[array1.Length+i]=array2[i];
            }
            return result;
        }
    }
}
