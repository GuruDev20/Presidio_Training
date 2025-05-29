using System.Threading.Tasks;
using FirstAPI.Misc;
using FirstAPI.Models.DTOs;

public class DoctorService : IDoctorService
{
    DoctorMapper doctorMapper;
    SpecialityMapper specialityMapper;
    private readonly IRepository<int, Doctor> _doctorRepository;
    private readonly IRepository<int, Speciality> _specialRepository;
    private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
    private readonly IOtherContextFunctions _otherContextFunctions;

    public DoctorService(IRepository<int, Doctor> doctorRepository, IRepository<int, Speciality> specialRepository, IRepository<int, DoctorSpeciality> doctorSpecialityRepository, IOtherContextFunctions otherContextFunctions)
    {
        doctorMapper = new DoctorMapper();
        specialityMapper = new SpecialityMapper();
        _doctorRepository = doctorRepository;
        _specialRepository = specialRepository;
        _doctorSpecialityRepository = doctorSpecialityRepository;
        _otherContextFunctions = otherContextFunctions;
    }

    public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctor)
    {
        try
        {
            var newDoctor = doctorMapper.MapDoctorAddRequestDoctor(doctor);
            newDoctor = await _doctorRepository.Add(newDoctor);
            if (newDoctor == null)
                throw new Exception("Could not add doctor");
            if (doctor.Specialities.Count() > 0)
            {
                int[] specialities = await MapAndAddSpeciality(doctor);
                for (int i = 0; i < specialities.Length; i++)
                {
                    var doctorSpeciality = specialityMapper.MapDoctorSpeciality(newDoctor.Id, specialities[i]);
                    doctorSpeciality = await _doctorSpecialityRepository.Add(doctorSpeciality);
                }
            }
            return newDoctor;

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

    }

    private async Task<int[]> MapAndAddSpeciality(DoctorAddRequestDto doctor)
    {
        int[] specialityIds = new int[doctor.Specialities.Count()];
        IEnumerable<Speciality> existingSpecialities = null;
        try
        {
            existingSpecialities = await _specialRepository.GetAll();
        }
        catch (Exception e)
        {

        }
        int count = 0;
        foreach (var item in doctor.Specialities)
        {
            Speciality speciality = null;
            if (existingSpecialities != null)
                speciality = existingSpecialities.FirstOrDefault(s => s.Name.ToLower() == item.Name.ToLower());
            if (speciality == null)
            {
                speciality = specialityMapper.MapSpecialityAddRequestSpeciality(item);
                speciality = await _specialRepository.Add(speciality);
            }
            specialityIds[count] = speciality.Id;
            count++;
        }
        return specialityIds;
    }

    public Task<Doctor> GetDoctorByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public  async Task<ICollection<DoctorsBySpecialityResponseDTO>>  GetDoctorsBySpecialityAsync(string specialityName)
    {
        var result = await _otherContextFunctions.GetDoctorBySpeciality(specialityName);
        return result;

    }

    Task<IEnumerable<Doctor>> IDoctorService.GetDoctorsBySpecialityAsync(string specialityName)
    {
        throw new NotImplementedException();
    }
}