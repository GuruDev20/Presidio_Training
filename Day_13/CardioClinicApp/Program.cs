using CardioClinicApp.Interfaces;
using CardioClinicApp.Models;
using CardioClinicApp.Repositories;
using CardioClinicApp.Services;
using CardioClinicApp.UI;

namespace CardioClinicApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            IRepository<int, Appointment> appointmentRepository = new AppointmentRepository();

            IAppointmentService appointmentService = new AppointmentService(appointmentRepository);

            Menu appointmentUI = new Menu(appointmentService);

            appointmentUI.Display();

        }
    }
}