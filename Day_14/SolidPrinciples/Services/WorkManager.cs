using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidPrinciples.Interfaces;

namespace SolidPrinciples.Services
{
    public class Human:IWorkable,IEatable
    {
        public void Work() => Console.WriteLine("Human Working");
        public void Eat() => Console.WriteLine("Human Eating");
    }
    public class Robot : IWorkable
    {
        public void Work() => Console.WriteLine("Robot Working");
    }
}
