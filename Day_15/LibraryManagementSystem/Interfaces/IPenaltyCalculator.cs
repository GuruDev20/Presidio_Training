using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Interfaces
{
    public interface IPenaltyCalculator
    {
        double CalculatePenalty(DateTime dueDate, DateTime returnDate);
    }
}
