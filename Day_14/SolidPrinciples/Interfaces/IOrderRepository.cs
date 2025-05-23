using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidPrinciples.Models;

namespace SolidPrinciples.Interfaces
{
    public interface IOrderRepository
    {
        void Save(Order order);
    }
}
