using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidPrinciples.Services;
using SolidPrinciples.Utilities;

namespace SolidPrinciples.DemonStration
{
    public class SRP_Demo
    {
        public static void Run()
        {
            ReportService reportService=new ReportService();
            var report = reportService.CreateReport("SRP", "Single Responsibility Principle");
            
            FileLogger logger = new FileLogger();
            logger.Save(report);
        }
    }
}
