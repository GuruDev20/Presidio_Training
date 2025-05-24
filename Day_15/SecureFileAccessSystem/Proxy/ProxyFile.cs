using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureFileAccessSystem.Interfaces;
using SecureFileAccessSystem.Models;

namespace SecureFileAccessSystem.Proxy
{
    public class ProxyFile:IFile
    {
        private readonly User _user;
        private readonly File _readFile;

        public ProxyFile(User user)
        {
            _user = user;
            _readFile = new File();
        }

        public void Read()
        {
            switch (_user.Role.ToLower())
            {
                case "admin":
                    _readFile.Read();
                    break;
                case "user":
                    Console.WriteLine("[Access Limited] Reading file metadata...");
                    break;
                case "guest":
                default:
                    Console.WriteLine("[Access Denied] You do not have permission to read this file.");
                    break;
            }
        }
    }
}
