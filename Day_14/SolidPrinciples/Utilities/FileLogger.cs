using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidPrinciples.Interfaces;
using SolidPrinciples.Models;

namespace SolidPrinciples.Utilities
{
    public class FileLogger:IReportSaver,ILogger
    {
        public void Save(Report report)
        {
            Console.WriteLine($"Report Saved: {report.Title}-{report.Content}");
        }
        public void Log(string message)
        {
            Console.WriteLine($"Log: {message}");
        }
    }
}
