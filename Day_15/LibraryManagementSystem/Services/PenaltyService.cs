using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{ 
    using LibraryManagementSystem.Interfaces;

    public class StandardPenaltyCalculator : IPenaltyCalculator
    {
        public double CalculatePenalty(DateTime dueDate, DateTime returnDate)
        {
            if (returnDate <= dueDate) return 0;
            return (returnDate - dueDate).Days * 1.5;
        }
    }
}
