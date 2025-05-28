using FirstAPI.Models.DTOs;

public interface IDoctorService
{
    public Task<Doctor> GetDoctorByNameAsync(string name);
    public Task<IEnumerable<Doctor>> GetDoctorsBySpecialityAsync(string specialityName);
    public Task<Doctor> AddDoctorAsync(DoctorAddRequestDto doctor);
}