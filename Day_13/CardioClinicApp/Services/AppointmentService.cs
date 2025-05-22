using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardioClinicApp.Interfaces;
using CardioClinicApp.Models;

namespace CardioClinicApp.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<int, Appointment> _appointmentsRepository;

        public AppointmentService(IRepository<int, Appointment> appointmentsRepository)
        {
            _appointmentsRepository = appointmentsRepository;
        }

        public int AddAppointment(Appointment appointment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(appointment.PatientName))
                {
                    Console.WriteLine("Validation Error: Patient Name cannot be empty.");
                    return -1;
                }
                if (appointment.PatientAge <= 0)
                {
                    Console.WriteLine("Validation Error: Patient Age must be positive.");
                    return -1;
                }
                if (appointment.AppointmentDate < DateTime.Now) 
                {
                    Console.WriteLine("Validation Error: Appointment Date cannot be in the past.");
                    return -1;
                }
                if (string.IsNullOrWhiteSpace(appointment.Reason))
                {
                    Console.WriteLine("Validation Error: Reason for visit cannot be empty.");
                    return -1;
                }

                var result = _appointmentsRepository.Add(appointment);
                if (result != null)
                {
                    return result.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error adding appointment: {e.Message}");
            }
            return -1;
        }

        public List<Appointment>? SearchAppointments(AppointmentSearchModel searchModel)
        {
            try
            {
                ICollection<Appointment>? appointments = _appointmentsRepository.GetAll();

                // chain filtering
                appointments = SearchByPatientName(appointments, searchModel.PatientName);
                appointments = SearchByAppointmentDate(appointments, searchModel.AppointmentDate);
                appointments = SearchByAgeRange(appointments, searchModel.AgeRange);

                if (appointments != null && appointments.Count > 0)
                {
                    return appointments.ToList();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error during appointment search: {e.Message}");
            }
            return null;
        }

        private ICollection<Appointment>? SearchByPatientName(ICollection<Appointment>? appointments, string? patientName)
        {
            if (string.IsNullOrWhiteSpace(patientName) || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }

            return appointments
                .Where(a => a.PatientName != null && a.PatientName.ToLower().Contains(patientName.ToLower()))
                .ToList();
        }

        private ICollection<Appointment>? SearchByAppointmentDate(ICollection<Appointment>? appointments, DateTime? appointmentDate)
        {

            if (appointmentDate == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            // Just comparing with Date for simplicity
            return appointments
                .Where(a => a.AppointmentDate.Date == appointmentDate.Value.Date)
                .ToList();
        }

        private ICollection<Appointment>? SearchByAgeRange(ICollection<Appointment>? appointments, (int Min, int Max)? ageRange)
        {
            if (!ageRange.HasValue || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }

            return appointments
                .Where(a => a.PatientAge >= ageRange.Value.Min && a.PatientAge <= ageRange.Value.Max)
                .ToList();
        }

        public int AddAppointment(string v, Appointment appointment)
        {
            throw new NotImplementedException();
        }
    }
}
