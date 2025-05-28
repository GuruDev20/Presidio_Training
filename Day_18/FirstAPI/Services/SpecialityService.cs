
public class SpecialityService : ISpecialityService
{
    private readonly IRepository<int,Speciality> _specialityRepository;
    public SpecialityService(IRepository<int, Speciality> specialityRepository)
    {
        _specialityRepository = specialityRepository;
    }

    public Task<Speciality> AddSpecialityAsync(Speciality speciality)
    {
        if (speciality == null)
        {
            throw new ArgumentNullException(nameof(speciality), "Speciality cannot be null.");
        }
        return _specialityRepository.Add(speciality);
    }

    public Task<Speciality> DeleteSpecialityAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid speciality ID.", nameof(id));
        }
        return _specialityRepository.Delete(id);
    }

    public Task<IEnumerable<Speciality>> GetAllSpecialitiesAsync()
    {
        return _specialityRepository.GetAll();
    }

    public Task<Speciality> GetSpecialityByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid speciality ID.", nameof(id));
        }
        return _specialityRepository.Get(id);
    }

    public Task<Speciality> UpdateSpecialityAsync(int id, Speciality speciality)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid speciality ID.", nameof(id));
        }
        if (speciality == null)
        {
            throw new ArgumentNullException(nameof(speciality), "Speciality cannot be null.");
        }
        if (id != speciality.Id)
        {
            throw new ArgumentException("Speciality ID mismatch.", nameof(speciality));
        }
        return _specialityRepository.Update(id, speciality);
    }
}