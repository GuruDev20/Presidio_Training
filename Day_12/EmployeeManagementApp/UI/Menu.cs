using System;
using EmployeeManagementApp.Services;

namespace EmployeeManagementApp.UI
{
    public class Menu
    {
        private EmployeeManager manager=new EmployeeManager();
        private Promotion promo=new Promotion();
        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add Employee");
                Console.WriteLine("2. Display All Employees");
                Console.WriteLine("3. Modify Employee");
                Console.WriteLine("4. Delete Employee");
                Console.WriteLine("5. Find Employee by ID");
                Console.WriteLine("6. Sort by Salary");
                Console.WriteLine("7. Find by Name");
                Console.WriteLine("8. Find Elder Than");
                Console.WriteLine("9. Promotion List Input");
                Console.WriteLine("10. Promotion Position");
                Console.WriteLine("11. Sorted Promotion List");
                Console.WriteLine("12. Exit");
                Console.Write("Choice: ");
                string choice=Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":manager.AddEmployee();break;
                        case "2":manager.DisplayAll();break;
                        case "3":manager.ModifyEmployee();break;
                        case "4":manager.DeleteEmployee();break;
                        case "5":manager.DisplayById();break;
                        case "6":manager.SortBySalary();break;
                        case "7":manager.FindByName();break;
                        case "8":manager.FindElderThan();break;
                        case "9":promo.ReadPromotionList();break;
                        case "10":promo.FindPromotionPosition();break;
                        case "11":promo.DisplaySorted();break;
                        case "12":return;
                        default:Console.WriteLine("Invalid Choice");break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
