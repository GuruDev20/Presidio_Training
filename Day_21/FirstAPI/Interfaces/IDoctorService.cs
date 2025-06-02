using FirstAPI.Models.DTOs;

public interface IDoctorService
{
    public Task<Doctor> GetDoctorByName(string name);
    public Task<ICollection<DoctorsBySpecialityResponseDTO>> GetDoctorsBySpeciality(string specialityName);
    public Task<Doctor> AddDoctor(DoctorAddRequestDto doctor);
}