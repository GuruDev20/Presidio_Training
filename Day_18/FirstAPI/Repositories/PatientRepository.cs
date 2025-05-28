using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

public class PatientRepository : Repository<int, Patient>
{
    public PatientRepository(ClinicContext clinicContext) : base(clinicContext) { }

    public override async Task<Patient> Get(int key)
    {
        var patient = await _clinicContext.Patients.SingleOrDefaultAsync(p => p.Id == key);
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }
        return patient;
    }

    public override async Task<IEnumerable<Patient>> GetAll()
    {
        var patients = await _clinicContext.Patients.ToListAsync();
        if (patients == null || !patients.Any())
        {
            throw new KeyNotFoundException("No patients found");
        }
        return patients;
    }
}
