using SecureFileAccessSystem.Models;
using SecureFileAccessSystem.Proxy;

namespace SecureFileAccessSystem
{
    class Program
    {
        static void Main()
        {
            List<User> users = new List<User>
            {
                new User("Alice", "Admin"),
                new User("Mikey", "User"),
                new User("Bob", "Guest")
            };

            foreach (var user in users)
            {
                Console.WriteLine($"\nUser: {user.Username} | Role: {user.Role}");
                var proxy = new ProxyFile(user);
                proxy.Read();
            }
        }
    }
}
