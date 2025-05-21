using EmployeeApp.Interfaces;
using EmployeeApp.Models;
using EmployeeApp.Repositories;
using EmployeeApp;
using EmployeeApp.Services;

namespace WholeApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {

            IRepository<int, Employee> employeeRepository = new EmployeeRepository();
            IEmployeeService employeeService = new EmployeeService(employeeRepository);
            ManageEmployee manageEmployee = new ManageEmployee(employeeService);
            manageEmployee.Start();
        }
    }
}