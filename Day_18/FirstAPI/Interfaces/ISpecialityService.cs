public interface ISpecialityService
{
    public Task<Speciality> GetSpecialityByIdAsync(int id);
    public Task<IEnumerable<Speciality>> GetAllSpecialitiesAsync();
    public Task<Speciality> AddSpecialityAsync(Speciality speciality);
    public Task<Speciality> UpdateSpecialityAsync(int id, Speciality speciality);
    public Task<Speciality> DeleteSpecialityAsync(int id);
}