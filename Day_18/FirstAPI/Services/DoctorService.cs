using FirstAPI.Models.DTOs;

public class DoctorService : IDoctorService
{
    private readonly IRepository<int, Doctor> _doctorRepository;
    private readonly IRepository<int, Speciality> _specialRepository;
    private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
    public DoctorService(IRepository<int, Doctor> doctorRepository, IRepository<int, Speciality> specialRepository, IRepository<int, DoctorSpeciality> doctorSpecialityRepository)
    { 
        _doctorRepository = doctorRepository;
        _specialRepository = specialRepository;
        _doctorSpecialityRepository = doctorSpecialityRepository;
    }

    public async Task<Doctor> AddDoctorAsync(DoctorAddRequestDto doctordto)
    {
        var doctor = new Doctor
        {
            Name = doctordto.Name,
            Status = "Active",
            YearsOfExperience = doctordto.YearsOfExperience,
        };
        var newDoctor = await _doctorRepository.Add(doctor);
        if (doctordto.Specialities != null)
        {
            foreach (var spec in doctordto.Specialities)
            {
                var existingSpec = (await _specialRepository.GetAll()).FirstOrDefault(s => s.Name.ToLower() == spec.Name.ToLower());
                if (existingSpec != null)
                {
                    await _doctorSpecialityRepository.Add(new DoctorSpeciality
                    {
                        DoctorId = newDoctor.Id,
                        SpecialityId = existingSpec.Id
                    });
                }
            }
        }
        return newDoctor;
    }

    public async Task<Doctor> GetDoctorByNameAsync(string name)
    {
        var allDoctors = await _doctorRepository.GetAll();
        var doctor = allDoctors.FirstOrDefault(d => d.Name.ToLower() == name.ToLower());
        if (doctor == null)
        {
            throw new KeyNotFoundException($"Doctor with name {name} not found.");
        }
        return doctor;
    }

    public async Task<IEnumerable<Doctor>> GetDoctorsBySpecialityAsync(string specialityName)
    {
        var specialities = await _specialRepository.GetAll();
        var speciality = specialities.FirstOrDefault(s => s.Name.ToLower() == specialityName.ToLower());
        if (speciality == null)
        {
            throw new KeyNotFoundException($"Speciality {specialityName} not found.");
        }
        var doctorSpecialities = await _doctorSpecialityRepository.GetAll();
        var doctorIds = doctorSpecialities
            .Where(ds => ds.SpecialityId == speciality.Id)
            .Select(ds => ds.DoctorId)
            .ToList();
        var allDoctors = await _doctorRepository.GetAll();
        var doctors = allDoctors.Where(d => doctorIds.Contains(d.Id)).ToList();
        if (doctors.Count == 0)
        {
            throw new KeyNotFoundException($"No doctors found for speciality {specialityName}.");
        }
        return doctors;
    }
}