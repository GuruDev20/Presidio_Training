
public class PatientService : IPatientService
{
    private readonly IRepository<int, Patient> _patientRepository;
    public PatientService(IRepository<int, Patient> patientRepository)
    {
        _patientRepository = patientRepository;
    }
    public Task<Patient> AddPatientAsync(Patient patient)
    {
        if (patient == null)
        {
            throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
        }
        return _patientRepository.Add(patient);
    }

    public Task<Patient> DeletePatientAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid patient ID.", nameof(id));
        }
        return _patientRepository.Delete(id);
    }

    public Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        return _patientRepository.GetAll();
    }

    public Task<Patient> GetPatientByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid patient ID.", nameof(id));
        }
        return _patientRepository.Get(id);
    }

    public Task<Patient> UpdatePatientAsync(int id, Patient patient)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid patient ID.", nameof(id));
        }
        if (patient == null)
        {
            throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
        }
        if (id != patient.Id)
        {
            throw new ArgumentException("Patient ID mismatch.", nameof(patient));
        }
        return _patientRepository.Update(id, patient);
    }
}