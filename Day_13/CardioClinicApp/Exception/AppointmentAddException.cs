using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardioClinicApp.Exception
{
    public class AppointmentAddException : Exception
    {
        public AppointmentAddException(string message) : base(message)
        {
        }
    }
}
