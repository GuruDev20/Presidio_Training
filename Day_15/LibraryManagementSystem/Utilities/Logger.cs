using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Utilities
{ 
    using LibraryManagementSystem.Interfaces;

    public class Logger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"[Log]: {message}");
    }
}
