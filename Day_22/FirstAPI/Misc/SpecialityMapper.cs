using FirstAPI.Models.DTOs;

public class SpecialityMapper
{
    public Speciality? MapSpecialityAddRequestSpeciality(SpecialityAddRequestDto specialityAddRequestDto)
    {
        Speciality speciality = new();
        speciality.Name = specialityAddRequestDto.Name;
        return speciality;
    }
    public DoctorSpeciality MapDoctorSpeciality(int doctorId,int specialityId)
    {
        DoctorSpeciality doctorSpeciality = new();
        doctorSpeciality.DoctorId = doctorId;
        doctorSpeciality.SpecialityId = specialityId;
        return doctorSpeciality;
    }
}