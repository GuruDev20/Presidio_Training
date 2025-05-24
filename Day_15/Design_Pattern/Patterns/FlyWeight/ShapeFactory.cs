using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.FlyWeight
{
    public class ShapeFactory
    {
        private static readonly Dictionary<string, IShape> shapes = new();

        public static IShape GetShape(string shapeType)
        {
            if (!shapes.ContainsKey(shapeType))
            {
                shapes[shapeType] = new Circle();
                Console.WriteLine("Shape created and added to pool.");
            }
            else
            {
                Console.WriteLine("Shape reused from pool.");
            }

            return shapes[shapeType];
        }
    }
}
