using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

public class DoctorSpecialityRepository : Repository<int, DoctorSpeciality>
{
    public DoctorSpecialityRepository(ClinicContext clinicContext) : base(clinicContext) { }

    public override async Task<DoctorSpeciality> Get(int key)
    {
        var doctorSpeciality = await _clinicContext.DoctorSpecialities
            .Include(ds => ds.Doctor)
            .Include(ds => ds.Speciality)
            .SingleOrDefaultAsync(ds => ds.SerialNumber == key);

        if (doctorSpeciality == null)
        {
            throw new KeyNotFoundException("DoctorSpeciality not found");
        }
        return doctorSpeciality;
    }

    public override async Task<IEnumerable<DoctorSpeciality>> GetAll()
    {
        var doctorSpecialities = await _clinicContext.DoctorSpecialities
            .Include(ds => ds.Doctor)
            .Include(ds => ds.Speciality)
            .ToListAsync();

        if (!doctorSpecialities.Any())
        {
            throw new KeyNotFoundException("No DoctorSpecialities found");
        }
        return doctorSpecialities;
    }
}
