using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidPrinciples.Interfaces;
using SolidPrinciples.Services;

namespace SolidPrinciples.DemonStration
{
    public class ISP_Demo
    {
        public static void Run()
        {
            IWorkable humanWorker=new Human();
            IWorkable robotWorker=new Robot();
            IEatable humanEater=new Human();
            humanWorker.Work();
            humanEater.Eat();
            robotWorker.Work();
        }
    }
}
