using SolidPrinciples.Models;

namespace SolidPrinciples.Services
{
    public class AreaCalculator
    {
        public double Calculate(IShape shape) => shape.Area();
    }
}
