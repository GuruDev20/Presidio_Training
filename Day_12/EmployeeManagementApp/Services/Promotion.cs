using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Services
{
    public class Promotion
    {
        private List<string> promotionList=new List<string>();
        public void ReadPromotionList()
        {
            Console.WriteLine("Enter names (blank to stop): ");
            string name;
            while (true)
            {
                name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    break;
                }
                promotionList.Add(name.Trim());
            }
            if (promotionList.Count == 0)
            {
                Console.WriteLine("No names were added.");
                return;
            }
            Console.WriteLine("Initial Size: " + promotionList.Capacity);
            promotionList.TrimExcess();
            Console.WriteLine("Trimmed Size: " + promotionList.Capacity); 
        }
        public void FindPromotionPosition()
        {
            if (promotionList.Count == 0)
            {
                Console.WriteLine("Promotion list is empty.");
                return;
            }
            Console.WriteLine("Enter name to check position: ");
            string name=Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return;
            }
            int index=promotionList.IndexOf(name);
            if (index >= 0)
            {
                Console.WriteLine($"{name} is at position {index + 1}");
            }
            else
            {
                Console.WriteLine("Name not found");
            }
        }
        public void DisplaySorted()
        {
            if (promotionList.Count == 0)
            {
                Console.WriteLine("Promotion list is empty.");
                return;
            }
            var sorted = promotionList.OrderBy(n => n).ToList();
            Console.WriteLine("Sorted promotion list: ");
            sorted.ForEach(n => Console.WriteLine(n));
        }
    }
}
