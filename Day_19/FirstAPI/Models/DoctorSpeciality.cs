using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DoctorSpeciality
{
    public int SerialNumber { get; set; }
    public int DoctorId { get; set; }
    public int SpecialityId { get; set; }
    public Speciality? Speciality { get; set; } 
    public Doctor? Doctor { get; set; }
}