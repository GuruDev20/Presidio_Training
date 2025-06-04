public interface IPatientService
{
    Task<Patient> GetPatientById(int id);
    Task<IEnumerable<Patient>> GetAllPatients();
    Task<Patient> AddPatient(PatientAddRequestDto patient);
}