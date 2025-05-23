using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SolidPrinciples.Models
{
    public interface IShape
    {
        double Area();
    }
    public class Circle : IShape
    {
        public double Radius {  get; set; }
        public double Area()=>Math.PI*Radius*Radius;
    }
    public class Rectangle : IShape
    {
        public double Width {  get; set; }
        public double Height { get; set; }
        public double Area()=>Width*Height;
    }
}
