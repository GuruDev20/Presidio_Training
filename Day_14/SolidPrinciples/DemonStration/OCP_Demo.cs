using SolidPrinciples.Models;
using SolidPrinciples.Services;

namespace SolidPrinciples.DemonStration
{
    public class OCP_Demo
    {
        public static void Run()
        {
            var shapes = new IShape[]
            {
                new Circle{Radius=5},
                new Rectangle{Width=5, Height=6},
            };

            AreaCalculator areaCalculator = new AreaCalculator();
            foreach (var shape in shapes)
            {
                Console.WriteLine($"Area: {areaCalculator.Calculate(shape)}");
            }
        }
    }
}
