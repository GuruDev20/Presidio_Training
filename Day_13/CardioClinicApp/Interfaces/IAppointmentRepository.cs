using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardioClinicApp.Models;

namespace CardioClinicApp.Interfaces
{
    public interface IAppointmentRepository
    {
        int Add(Appointment appointment);
        List<Appointment> Search(AppointmentSearchModel searchModel);
    }
}
