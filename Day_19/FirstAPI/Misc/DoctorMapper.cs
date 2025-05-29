using FirstAPI.Models.DTOs;

namespace FirstAPI.Misc
{
    public class DoctorMapper
    {
        public Doctor? MapDoctorAddRequestDoctor(DoctorAddRequestDto doctorAddRequestDto)
        {
            Doctor doctor = new();
            doctor.Name= doctorAddRequestDto.Name;
            doctor.Status = "Active";
            doctor.YearsOfExperience = doctorAddRequestDto.YearsOfExperience;
            return doctor;
        }
    }
}