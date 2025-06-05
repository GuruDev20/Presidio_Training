using System.Threading.Tasks;
using AutoMapper;
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
    private readonly IEncryptionService _encryptionService;
    private readonly IMapper _mapper;
    private readonly IRepository<string, User> _userRepository;

    public DoctorService(IRepository<int, Doctor> doctorRepository, IRepository<int, Speciality> specialRepository, IRepository<int, DoctorSpeciality> doctorSpecialityRepository, IOtherContextFunctions otherContextFunctions, IEncryptionService encryptionService, IMapper mapper, IRepository<string, User> userRepository)
    {
        doctorMapper = new DoctorMapper();
        specialityMapper = new SpecialityMapper();
        _doctorRepository = doctorRepository;
        _specialRepository = specialRepository;
        _doctorSpecialityRepository = doctorSpecialityRepository;
        _otherContextFunctions = otherContextFunctions;
        _encryptionService = encryptionService;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctor)
    {
        try
        {
            var normalizedEmail = doctor.Email.Trim().ToLower();
            var existingUser = await _userRepository.Get(normalizedEmail);
            if (existingUser != null)
            {
                throw new Exception("A user with this email already exists.");
            }
            var user = _mapper.Map<DoctorAddRequestDto, User>(doctor);

            var encryptedData = await _encryptionService.EncryptData(new EncryptionModel
            {
                Data = doctor.Password,
            });
            user.Password = encryptedData.EncryptedData;
            user.HashKey = encryptedData.HashKey;
            user.Role = "Doctor";
            user = await _userRepository.Add(user);

            var newDoctor = doctorMapper.MapDoctorAddRequestDoctor(doctor);
            newDoctor = await _doctorRepository.Add(newDoctor);
            if (newDoctor == null)
                throw new Exception("Could not add doctor");
            if (doctor.Specialities?.Any() == true)
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

    public Task<Doctor> GetDoctorByName(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<DoctorsBySpecialityResponseDTO>> GetDoctorsBySpeciality(string specialityName)
    {
        var result = await _otherContextFunctions.GetDoctorBySpeciality(specialityName);
        return result;
    }
}