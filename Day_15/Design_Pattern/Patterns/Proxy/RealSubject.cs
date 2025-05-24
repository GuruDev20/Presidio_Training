using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.Proxy
{
    public class RealSubject:ISubject
    {
        public void Operation()
        {
            Console.WriteLine("RealSubject");
        }
    }
}
