using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DoctorSpeciality
{
    [Key]
    public int SerialNumber { get; set; }
    public int DoctorId { get; set; }
    public int SpecialtyId { get; set; }
    [ForeignKey("SpecialtyId")]
    public Speciality? Speciality { get; set; } 
    [ForeignKey("DoctorId")]
    public Doctor? Doctor { get; set; }
}