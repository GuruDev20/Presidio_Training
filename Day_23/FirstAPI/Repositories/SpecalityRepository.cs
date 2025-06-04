using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

public class SpecialityRepository : Repository<int, Speciality>
{
    public SpecialityRepository(ClinicContext clinicContext) : base(clinicContext) { }

    public override async Task<Speciality> Get(int key)
    {
        var speciality=await _clinicContext.Specialities.FindAsync(key);
        if (speciality == null)
        {
            throw new KeyNotFoundException("Speciality not found");
        }
        return speciality;
    }

    public override async Task<IEnumerable<Speciality>> GetAll()
    {
        var specialities = await _clinicContext.Specialities.ToListAsync();
        if (specialities == null || !specialities.Any())
        {
            throw new KeyNotFoundException("No specialities found");
        }
        return specialities;
    }
}
