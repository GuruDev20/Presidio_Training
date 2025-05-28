public interface IPatientService
{
    public Task<Patient> GetPatientByIdAsync(int id);
    public Task<IEnumerable<Patient>> GetAllPatientsAsync();
    public Task<Patient> AddPatientAsync(Patient patient);
    Task<Patient> UpdatePatientAsync(int id, Patient patient);
    Task<Patient> DeletePatientAsync(int id);
}