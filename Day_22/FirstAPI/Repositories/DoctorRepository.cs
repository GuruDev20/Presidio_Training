using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

public class DoctorRepository : Repository<int, Doctor>
{
    public DoctorRepository(ClinicContext clinicContext) : base(clinicContext) { }
    public override async Task<Doctor> Get(int key)
    {
        var doctor = await _clinicContext.Doctors.FindAsync(key);
        if (doctor == null)
        {
            throw new KeyNotFoundException("Doctor not found");
        }
        return doctor;
    }
    public override async Task<IEnumerable<Doctor>> GetAll()
    {
        var doctors = await _clinicContext.Doctors.ToListAsync();
        if (doctors == null || !doctors.Any())
        {
            throw new KeyNotFoundException("No doctors found");
        }
        return doctors;
    }
}