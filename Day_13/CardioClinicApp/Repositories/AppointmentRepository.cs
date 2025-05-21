using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardioClinicApp.Interfaces;
using CardioClinicApp.Models;

namespace CardioClinicApp.Repositories
{
    public class AppointmentRepository:IAppointmentRepository
    {
        private readonly List<Appointment> _appointments = new List<Appointment>();
        private int _nextId = 1;

        public int Add(Appointment appointment)
        {
            try
            {
                appointment.Id = _nextId++;
                _appointments.Add(appointment);
                return appointment.Id;
            }
            catch
            {
                throw new Exception("Failed to add appointment.");
            }
        }

        public List<Appointment> Search(AppointmentSearchModel searchModel)
        {
            List<Appointment> filtered = new List<Appointment>(_appointments);

            if (!string.IsNullOrWhiteSpace(searchModel.PatientName))
            {
                filtered = filtered.Where(a => a.PatientName.Contains(searchModel.PatientName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (searchModel.AppointmentDate.HasValue)
            {
                filtered = filtered.Where(a => a.AppointmentDate.Date == searchModel.AppointmentDate.Value.Date).ToList();
            }

            if (searchModel.AgeRange.HasValue)
            {
                filtered = filtered.Where(a => a.PatientAge >= searchModel.AgeRange.Value.Min &&a.PatientAge <= searchModel.AgeRange.Value.Max).ToList();
            }

            return filtered;
        }

    }
}
