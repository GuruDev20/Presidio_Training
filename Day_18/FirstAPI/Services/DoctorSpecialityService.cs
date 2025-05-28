
public class DoctorSpecialityService : IDoctorSpecialityService
{
    private readonly IRepository<int,DoctorSpeciality> _doctorSpecialityRepository;
    public DoctorSpecialityService(IRepository<int, DoctorSpeciality> doctorSpecialityRepository)
    {
        _doctorSpecialityRepository = doctorSpecialityRepository;
    }

    public Task<DoctorSpeciality> AddDoctorSpecialityAsync(DoctorSpeciality doctorSpeciality)
    {
        if (doctorSpeciality == null)
        {
            throw new ArgumentNullException(nameof(doctorSpeciality), "DoctorSpeciality cannot be null.");
        }
        return _doctorSpecialityRepository.Add(doctorSpeciality);
    }

    public Task<DoctorSpeciality> DeleteDoctorSpecialityAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid DoctorSpeciality ID.", nameof(id));
        }
        return _doctorSpecialityRepository.Delete(id);
    }

    public Task<IEnumerable<DoctorSpeciality>> GetAllDoctorSpecialitiesAsync()
    {
        return _doctorSpecialityRepository.GetAll();
    }

    public Task<DoctorSpeciality> GetDoctorSpecialityByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid DoctorSpeciality ID.", nameof(id));
        }
        return _doctorSpecialityRepository.Get(id);
    }

    public Task<DoctorSpeciality> UpdateDoctorSpecialityAsync(int id, DoctorSpeciality doctorSpeciality)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid DoctorSpeciality ID.", nameof(id));
        }
        if (doctorSpeciality == null)
        {
            throw new ArgumentNullException(nameof(doctorSpeciality), "DoctorSpeciality cannot be null.");
        }
        if (id != doctorSpeciality.DoctorId)
        {
            throw new ArgumentException("DoctorSpeciality ID mismatch.", nameof(doctorSpeciality));
        }
        return _doctorSpecialityRepository.Update(id, doctorSpeciality);
    }
}