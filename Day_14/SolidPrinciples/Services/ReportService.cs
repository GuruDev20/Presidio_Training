using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidPrinciples.Models;

namespace SolidPrinciples.Services
{
    public class ReportService
    {
        public Report CreateReport(string title,string content)
        {
            return new Report { Title= title, Content= content };
        }
    }
}
