using System;

namespace Tasks.TaskHandler
{
    public class Task11_ValidateSudokuBoard
    {
        public static void Run()
        {
            int[,] board = ReadSudokuBoard();

            bool areRowsValid = ValidateAllRows(board);
            bool areColumnsValid = ValidateAllColumns(board);
            bool areBlocksValid = ValidateAllBlocks(board);

            if (areRowsValid && areColumnsValid && areBlocksValid)
                Console.WriteLine("Sudoku board is valid.");
            else
                Console.WriteLine("Sudoku board is invalid.");
        }

        public static int[,] ReadSudokuBoard()
        {
            int[,] board = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                while (true)
                {
                    Console.WriteLine($"Enter 9 numbers for row {i + 1} (separated by spaces): ");
                    string[] input = Console.ReadLine().Split();
                    if (input.Length != 9)
                    {
                        Console.WriteLine("You must enter exactly 9 numbers.");
                        continue;
                    }
                    bool isValidRow = true;
                    for (int j = 0; j < 9; j++)
                    {
                        if (!int.TryParse(input[j], out board[i, j]) || board[i, j] < 1 || board[i, j] > 9)
                        {
                            Console.WriteLine("Invalid input. Enter digits from 1 to 9 only.");
                            isValidRow = false;
                            break;
                        }
                    }
                    if (isValidRow) break;
                }
            }

            return board;
        }

        public static bool ValidateAllRows(int[,] board)
        {
            for (int i = 0; i < 9; i++)
            {
                int[] row = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    row[j] = board[i, j];
                }
                if (!ValidateNineDigits(row))
                {
                    Console.WriteLine($"Row {i + 1} is invalid.");
                    return false;
                }
            }
            return true;
        }

        public static bool ValidateAllColumns(int[,] board)
        {
            for (int j = 0; j < 9; j++)
            {
                int[] column = new int[9];
                for (int i = 0; i < 9; i++)
                {
                    column[i] = board[i, j];
                }
                if (!ValidateNineDigits(column))
                {
                    Console.WriteLine($"Column {j + 1} is invalid.");
                    return false;
                }
            }
            return true;
        }

        public static bool ValidateAllBlocks(int[,] board)
        {
            for (int blockRow = 0; blockRow < 3; blockRow++)
            {
                for (int blockCol = 0; blockCol < 3; blockCol++)
                {
                    int[] block = new int[9];
                    int index = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            block[index++] = board[blockRow * 3 + i, blockCol * 3 + j];
                        }
                    }
                    if (!ValidateNineDigits(block))
                    {
                        Console.WriteLine($"3x3 Block starting at row {blockRow * 3 + 1}, column {blockCol * 3 + 1} is invalid.");
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ValidateNineDigits(int[] group)
        {
            bool[] seen = new bool[9];
            foreach (int num in group)
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
