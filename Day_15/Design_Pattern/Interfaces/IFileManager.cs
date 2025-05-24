using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Interfaces
{
    public interface IFileManager
    {
        void Write(string content);
        string Read();
        void Close();
    }
}
