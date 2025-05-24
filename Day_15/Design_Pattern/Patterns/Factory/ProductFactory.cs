using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.Factory
{
    public class ProductFactory
    {
        public static IProduct GetProduct(string type)
        {
            return type switch
            {
                "A" => new ProductA(),
                "B" => new ProductB()
            };
        }
    }
}
