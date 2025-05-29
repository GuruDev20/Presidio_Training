using FirstAPI.Models.DTOs;

public interface IDoctorService
{
    public Task<Doctor> GetDoctorByNameAsync(string name);
    public Task<IEnumerable<DoctorsBySpecialityResponseDTO>> GetDoctorsBySpecialityAsync(string specialityName);
    public Task<Doctor> AddDoctor(DoctorAddRequestDto doctor);
}