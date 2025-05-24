using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureFileAccessSystem.Interfaces;

namespace SecureFileAccessSystem.Proxy
{
    public class File:IFile
    {
        public void Read()
        {
            Console.WriteLine("[Access Granted] Reading sensitive file content...");
        }
    }
}
