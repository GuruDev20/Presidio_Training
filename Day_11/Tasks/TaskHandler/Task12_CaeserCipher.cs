using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task12_CaeserCipher
    {
        public static void Run()
        {
            string message = ReadMessage();
            int shift = ReadShift();

            string encrypted = Encrypt(message, shift);
            string decrypted = Decrypt(encrypted, shift);

            Console.WriteLine($"Encrypted: {encrypted}");
            Console.WriteLine($"Decrypted: {decrypted}");
        }

        public static string ReadMessage()
        {
            while (true)
            {
                Console.Write("Enter a lowercase message: ");
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && input.All(c => c >= 'a' && c <= 'z'))
                {
                    return input;
                }

                Console.WriteLine("Invalid input. Use only lowercase letters with no spaces.");
            }
        }

        public static int ReadShift()
        {
            Console.Write("Enter shift value: ");
            int shift;
            while (!int.TryParse(Console.ReadLine(), out shift))
            {
                Console.Write("Invalid shift. Please enter a valid integer: ");
            }
            return shift;

        }
        public static string Encrypt(string message,int shift)
        {
            return new string(message.Select(c => (char)(((c - 'a' + shift) % 26) + 'a')).ToArray());
        }

        public static string Decrypt(string message, int shift)
        {
            return new string(message.Select(c => (char)(((c - 'a' - shift + 26) % 26) + 'a')).ToArray());
        }
    }
}
