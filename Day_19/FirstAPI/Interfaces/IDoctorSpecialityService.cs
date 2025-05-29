public interface IDoctorSpecialityService
{
    public Task<DoctorSpeciality> GetDoctorSpecialityByIdAsync(int id);
    public Task<IEnumerable<DoctorSpeciality>> GetAllDoctorSpecialitiesAsync();
    public Task<DoctorSpeciality> AddDoctorSpecialityAsync(DoctorSpeciality doctorSpeciality);
    public Task<DoctorSpeciality> UpdateDoctorSpecialityAsync(int id, DoctorSpeciality doctorSpeciality);
    public Task<DoctorSpeciality> DeleteDoctorSpecialityAsync(int id);
}