using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.FlyWeight
{
    public class Circle:IShape
    {
        private readonly string _shapeType = "Circle";
        public void Draw(string color)
        {
            Console.WriteLine($"Drawing {_shapeType} of color {color}");
        }
    }
}
