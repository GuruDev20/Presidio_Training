using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardioClinicApp.Models;
using CardioClinicApp.Services;

namespace CardioClinicApp.UI
{
    public class Menu
    {
        private readonly AppointmentService _service;

        public Menu(AppointmentService service)
        {
            _service = service;
        }

        public void Display()
        {
            while (true)
            {
                Console.WriteLine("\n--- Cardiologist Appointment System ---");
                Console.WriteLine("1. Add Appointment");
                Console.WriteLine("2. Search Appointments");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AddAppointment();
                        break;
                    case "2":
                        SearchAppointments();
                        break;
                    case "3":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        private void AddAppointment()
        {
            Console.Write("Enter patient name: ");
            var name = Console.ReadLine();

            Console.Write("Enter age: ");
            if (!int.TryParse(Console.ReadLine(), out int age))
            {
                Console.WriteLine("Invalid age.");
                return;
            }

            Console.Write("Enter appointment date (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.Write("Enter reason: ");
            var reason = Console.ReadLine();

            try
            {
                var id = _service.AddAppointment(name!, age, date, reason!);
                Console.WriteLine($"Appointment added. ID: {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void SearchAppointments()
        {
            var searchModel = new AppointmentSearchModel();

            Console.Write("Search by patient name (leave blank to skip): ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                searchModel.PatientName = name;
            }
            Console.Write("Search by appointment date (yyyy-mm-dd or blank): ");
            var dateInput = Console.ReadLine();
            if (DateTime.TryParse(dateInput, out var date))
            {
                searchModel.AppointmentDate = date;
            }
            Console.Write("Search by age range (e.g. 25-35 or blank): ");
            var ageRange = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ageRange))
            {
                if (ageRange.Contains('-'))
                {
                    var parts = ageRange.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[0], out var min) && int.TryParse(parts[1], out var max))
                    {
                        searchModel.AgeRange = (min, max);
                    }
                    else
                    {
                        Console.WriteLine("Invalid age range format. Please use format like 25-35.");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid age range format. Please use format like 25-35.");
                    return; 
                }
            }

            try
            {
                var results = _service.SearchAppointments(searchModel);
                if (results.Count == 0)
                {
                    Console.WriteLine("No appointments found.");
                }
                else
                {
                    Console.WriteLine("\nMatching Appointments:");
                    foreach (var app in results)
                    {
                        Console.WriteLine($"ID: {app.Id}, Name: {app.PatientName}, Age: {app.PatientAge}, Date: {app.AppointmentDate:yyyy-MM-dd}, Reason: {app.Reason}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search error: {ex.Message}");
            }
        }
    }
}
