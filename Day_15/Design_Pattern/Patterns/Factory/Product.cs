using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.Factory
{
    public class ProductA:IProduct
    {
        public void Display() => Console.WriteLine("Product A Selected");
    }
    public class ProductB : IProduct
    {
        public void Display() => Console.WriteLine("Product B Selected");
    }
}
