
using AutoMapper;

public class PatientService : IPatientService
{
    private readonly IRepository<int, Patient> _patientRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IRepository<string, User> _userRepository;
    private readonly IMapper _mapper;
    public PatientService(IRepository<int, Patient> patientRepository, IEncryptionService encryptionService, IRepository<string, User> userRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _encryptionService = encryptionService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Patient> AddPatient(PatientAddRequestDto patientDto)
    {
        try
        {
            var normalizedEmail = patientDto.Email.Trim().ToLower();
            var existingUser = await _userRepository.Get(normalizedEmail);
            if (existingUser != null)
            {
                throw new Exception("A user with this email already exists.");
            }
            var user = _mapper.Map<PatientAddRequestDto, User>(patientDto);
            var encryptedData = await _encryptionService.EncryptData(new EncryptionModel
            {
                Data = patientDto.Password,
            });

            user.Password = encryptedData.EncryptedData;
            user.HashKey = encryptedData.HashKey;
            user.Role = "Patient";

            await _userRepository.Add(user);
            var newPatient = _mapper.Map<PatientAddRequestDto, Patient>(patientDto);
            var savedPatient = await _patientRepository.Add(newPatient);

            return savedPatient;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<IEnumerable<Patient>> GetAllPatients()
    {
        throw new NotImplementedException();
    }

    public Task<Patient> GetPatientById(int id)
    {
        throw new NotImplementedException();
    }
}