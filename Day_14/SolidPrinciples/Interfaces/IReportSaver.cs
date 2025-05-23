using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidPrinciples.Models;

namespace SolidPrinciples.Interfaces
{
    public interface IReportSaver
    {
        void Save(Report report);
    }
}
