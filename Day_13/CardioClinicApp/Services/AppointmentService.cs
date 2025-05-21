using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardioClinicApp.Interfaces;
using CardioClinicApp.Models;

namespace CardioClinicApp.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        public int AddAppointment(string name, int age, DateTime date, string reason)
        {
            var appointment = new Appointment
            {
                PatientName = name,
                PatientAge = age,
                AppointmentDate = date,
                Reason = reason
            };

            return _repository.Add(appointment);
        }

        public List<Appointment> SearchAppointments(AppointmentSearchModel model)
        {
            try
            {
                return _repository.Search(model);
            }
            catch
            {
                throw new Exception("Error while searching appointments.");
            }
        }
    }
}
