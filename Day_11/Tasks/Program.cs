using System;
using Tasks.TaskHandler;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Enter Task no:");
        string input=Console.ReadLine();
        switch (input)
        {
            case "1":
                Task1_GreetUser.Run();
                break;
            case "2":
                Task2_LargestNumber.Run();
                break;
            case "3":
                Task3_Calculator.Run();
                break;
            case "4":
                Task4_Login.Run();
                break;
            case "5":
                Task5_Divisible7.Run();
                break;
            case "6":
                Task6_FrequencyCounter.Run();
                break;
            case "7":
                Task7_LeftRotate.Run();
                break;
            case "8":
                Task8_MergeArrays.Run();
                break;
            case "9":
                Task9_BullsAndCows.Run();
                break;
            case "10":
                Task10_ValidateSudokuRow.Run();
                break;
            case "11":
                Task11_ValidateSudokuBoard.Run();
                break;
            case "12":
                Task12_CaeserCipher.Run();
                break;
            default:
                Console.WriteLine("Invalid choice");
                break;
        }
    }
}