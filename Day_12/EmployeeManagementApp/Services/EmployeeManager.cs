using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagementApp.Models;

namespace EmployeeManagementApp.Services
{
    public class EmployeeManager
    {
        private Dictionary<int,Employee> employees=new Dictionary<int, Employee>();

        private int SafeReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int value))
                {
                    return value;
                }
                Console.WriteLine("Invalid number. Try again.");
            }
        }

        private double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (double.TryParse(input, out double value) && value >= 0)
                {
                    return value;
                }
                Console.WriteLine("Invalid salary. Must be >= 0.");
            }
        }


        public void AddEmployee()
        {
            var emp=new Employee();
            emp.ReadDetails();
            if (!employees.ContainsKey(emp.Id))
            {
                employees.Add(emp.Id, emp);
                Console.WriteLine("Employee Added successfully");
            }
            else
            {
                Console.WriteLine("ID already exists");
            }
        }
        public void DisplayAll()
        {
            if (employees.Count == 0)
            {
                Console.WriteLine("No employees");
                return;
            }
            foreach(var emp in employees.Values)
            {
                emp.Display();
            }
        }
        public void ModifyEmployee()
        {
            int id = SafeReadInt("Enter ID to modify: ");
            if (employees.TryGetValue(id, out var emp))
            {
                Console.Write("Enter new name: ");
                string name = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(name))
                {
                    Console.Write("Name cannot be empty. Enter again: ");
                    name = Console.ReadLine();
                }

                emp.Name = name;
                emp.Age = SafeReadInt("Enter new age (18-65): ");
                emp.Salary = ReadDouble("Enter new salary (>=0): ");
                Console.WriteLine("Employee updated.");
            }
            else
            {
                Console.WriteLine("Not found.");
            }
        }
        public void DeleteEmployee()
        {
            int id = SafeReadInt("Enter ID to delete: ");
            if (employees.Remove(id))
            {
                Console.WriteLine("Employee Deleted");
            }
            else
            {
                Console.WriteLine("Not found");
            }
        }
        public void DisplayById()
        {
            Console.WriteLine("Enter ID to search: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var result = employees.Where(e => e.Key == id).Select(e => e.Value).FirstOrDefault();
                if (result != null)
                {
                    result.Display();
                }
                else
                {
                    Console.WriteLine("Not found");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid integer ID.");
            }
        }
        public void SortBySalary()
        {
            var list = employees.Values.ToList();
            list.Sort();
            Console.WriteLine("Sorted by Salary: ");
            foreach (var emp in list)
            {
                emp.Display();
            }
        }
        public void FindByName()
        {
            Console.WriteLine("Enter name: ");
            string name=Console.ReadLine();
            var list=employees.Values.Where(e=>e.Name==name).ToList();
            if (list.Count == 0)
            {
                Console.WriteLine("No employee found");
            }
            else
            {
                list.ForEach(e=>e.Display());
            }
        }
        public void FindElderThan()
        {
            int id = SafeReadInt("Enter ID to compare: ");
            if (employees.TryGetValue(id, out var refEmp))
            {
                var list = employees.Values.Where(e => e.Age > refEmp.Age).ToList();
                if (list.Count == 0)
                {
                    Console.WriteLine($"No employees are older than {refEmp.Name}.");
                }
                else
                {
                    Console.WriteLine($"Employees older than {refEmp.Name}:");
                    list.ForEach(e => e.Display());
                }
            }
            else
            {
                Console.WriteLine("Employee not found");
            }
        }

    }
}
