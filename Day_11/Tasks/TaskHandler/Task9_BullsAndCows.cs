using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task9_BullsAndCows
    {
        public static readonly string secret = "GAME";
        public static void Run()
        {
            int attempts = 0;
            while (true)
            { 
                string guess =ReadGuess();
                attempts++;
                int bulls, cows;
                GetBullsAndCows(guess,out bulls, out cows);
                Console.WriteLine($"{bulls} Bulls {cows} Cows");
                if (bulls == 4)
                {
                    Console.WriteLine($"Correct Attempts:{attempts}");
                    break;
                }
            }
        }

        public static string ReadGuess()
        {
            while (true)
            {
                Console.Write("Enter your 4-letter guess: ");
                string input = Console.ReadLine().ToUpper();
                if (input.Length == 4 && input.All(char.IsLetter))
                {
                    return input;
                }
                Console.WriteLine("Invalid input. Please enter exactly 4 letters.");
            }
        }


        public static void GetBullsAndCows(string guess,out int bulls,out int cows)
        {   
            bulls = 0; cows=0;
            if (guess.Length != secret.Length)
            {
                Console.WriteLine("Guess length does not match secret length.");
                return;
            }
            bool[] secretUsed = new bool[4];
            bool[] guessUsed = new bool[4];
            for(int i = 0; i < 4; i++)
            {
                if (guess[i] == secret[i])
                {
                    bulls++;
                    secretUsed[i] = true;
                    guessUsed[i] = true;
                }
            }
            for(int i = 0; i < 4; i++)
            {
                if (guessUsed[i]) continue;
                for(int j = 0; j < 4; j++)
                {
                    if (!secretUsed[j] && guess[i] == secret[j])
                    {
                        cows++;
                        secretUsed[j] = true;
                        break;
                    }
                }
            }
        }
    }
}
