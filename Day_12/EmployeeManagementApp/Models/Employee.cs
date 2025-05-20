using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagementApp.Models
{
    public class Employee : IComparable<Employee>
    {
        public int Id { get; set; }
        public int Age {  get; set; }
        public string Name { get; set; }
        public double Salary {  get; set; }
        public Employee() { }
        public Employee(int id, int age, string name, double salary)
        {
            Id = id;
            Age = age;
            Name = name;
            Salary = salary;
        }
        private static HashSet<int> usedIds = new HashSet<int>();
        public void ReadDetails()
        {
            do
            {
                Id = ReadInt("Enter ID: ");
                if (usedIds.Contains(Id))
                {
                    Console.WriteLine("This ID already exists. Please enter a unique ID.");
                }
            } 
            while (usedIds.Contains(Id));
            usedIds.Add(Id);
            Console.WriteLine("Enter Name: ");
            Name = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(Name) || !IsValidName(Name))
            {
                Console.WriteLine("Invalid name. Only letters and spaces are allowed. Enter Name again: ");
                Name = Console.ReadLine();
            }

            Age = ReadInt("Enter Age: ");
            Salary = ReadDouble("Enter Salary: ", 0);
        }

        private bool IsValidName(string name)
        {
            return name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        private int ReadInt(string prompt)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out value))
                {
                    return value;
                }
                Console.WriteLine($"Invalid input. Please enter a valid number");
            }
        }

        private double ReadDouble(string prompt, double min = double.MinValue)
        {
            double value;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (double.TryParse(input, out value) && value >= min)
                {
                    return value;
                }
                Console.WriteLine($"Invalid input. Please enter a valid number >= {min}.");
            }
        }
        public void Display()
        {
            Console.WriteLine($"ID: {Id} | Name: {Name} | Age: {Age} | Salary: {Salary}\n");
        }
        public int CompareTo(Employee other)
        {
            return this.Salary.CompareTo(other.Salary);
        }
    }
}
