using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidPrinciples.Models;

namespace SolidPrinciples.DemonStration
{
    public class LSP_Demo
    {
        public static void Run()
        {
            Bird sparrow = new Sparrow { Name = "Jack" };
            Bird penguin = new Penguin { Name = "Brave" };
            if (sparrow is IFlyable flyer)
            {
                flyer.Fly();
            }
            if (penguin is Penguin p)
            {
                p.Swim();
            }
        }
    }
}
