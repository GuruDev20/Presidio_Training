using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.Proxy
{
    public class ProxySubject:ISubject
    {
        private RealSubject? _realSubject;
        public void Operation()
        {
            if(_realSubject == null)
            {
                Console.WriteLine("Proxy : Starting Real subject");
                _realSubject = new RealSubject();
            }
            Console.WriteLine("Proxy : Repeating Real subject");
            _realSubject.Operation();
        }
    }
}
