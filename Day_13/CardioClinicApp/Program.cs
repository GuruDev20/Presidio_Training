using CardioClinicApp.Interfaces;
using CardioClinicApp.Repositories;
using CardioClinicApp.Services;
using CardioClinicApp.UI;

IAppointmentRepository repository = new AppointmentRepository();
var service = new AppointmentService(repository);
var menu = new Menu(service);

menu.Display();
