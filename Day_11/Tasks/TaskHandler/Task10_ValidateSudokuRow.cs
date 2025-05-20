using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task10_ValidateSudokuRow
    {
        public static void Run()
        {
            int[] row = ReadRow();
            bool isValid = ValidateRow(row);
            Console.WriteLine(isValid ? "Row is valid" : "Row is invalid");
        }

        public static int[] ReadRow()
        {
            int[] row = new int[9];
            for (int i = 0; i < 9; i++)
            {
                Console.Write($"Enter number {i + 1} (1-9): ");
                while (!int.TryParse(Console.ReadLine(), out row[i]) || row[i] < 1 || row[i] > 9)
                {
                    Console.Write("Invalid input. Enter a number from 1 to 9: ");
                }
            }
            return row;
        }

        public static bool ValidateRow(int[] row)
        {
            if (row == null || row.Length != 9)
            {
                Console.WriteLine("Row must contain exactly 9 numbers.");
                return false;
            }
            bool[] seen = new bool[9];
            foreach(int num in row)
            {
                if (seen[num - 1])
                {
                    return false;
                }
                seen[num - 1] = true;
            }
            return true;
        }
    }
}
