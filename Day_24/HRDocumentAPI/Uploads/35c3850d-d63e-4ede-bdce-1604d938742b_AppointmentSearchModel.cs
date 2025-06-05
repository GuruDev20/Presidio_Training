using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardioClinicApp.Models
{
    public class AppointmentSearchModel
    {
        public string? PatientName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public (int Min, int Max)? AgeRange { get; set; }
    }
}
