using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskHandler
{
    public class Task4_Login
    {
        public static void Run()
        {
            int maxAttempts = 3;
            for(int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                (string user, string password) = GetCredentials();
                if (isValid(user, password))
                {
                    Console.WriteLine("Success! Logged in.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Invalid credentials. Attempts left: {maxAttempts - attempt}");
                }
            }
            Console.WriteLine("Invalid attempts for 3 times. Exiting....");
        }

        public static (string,string) GetCredentials()
        {
            Console.WriteLine("Enter username:");
            string user=Console.ReadLine();
            Console.WriteLine("Enter password:");
            string password=Console.ReadLine();
            return (user, password);
        }

        public static bool isValid(string user, string password)
        {
            return user == "Admin" && password == "pass";
        }
    }
}
