using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidPrinciples.Models
{
    public abstract class Bird
    {
        public string Name { get; set; }
    }
    public interface IFlyable
    {
        void Fly();
    }
    public class Sparrow : Bird, IFlyable
    {
        public void Fly() => Console.WriteLine($"{Name} is Flying");
    }
    public class Penguin : Bird
    {
        public void Swim() => Console.WriteLine($"{Name} is swimming.");
    }
}
