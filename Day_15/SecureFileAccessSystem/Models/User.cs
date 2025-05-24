using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureFileAccessSystem.Models
{
    public class User
    {
        public string Username {  get; set; }
        public string Role {  get; set; }
        public User(string username, string role)
        {
            Username = username;
            Role = role;
        }
    }
}
