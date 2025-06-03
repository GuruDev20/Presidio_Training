
using System.Threading.Tasks;
using FirstAPI.Contexts;

public class OtherFunctionalitiesImplementation : IOtherContextFunctions
{
    private readonly ClinicContext _context;
    public OtherFunctionalitiesImplementation(ClinicContext context)
    {
        _context = context;
    }
    public async Task<ICollection<DoctorsBySpecialityResponseDTO>> GetDoctorBySpeciality(string speciality)
    {
        var result = await _context.GetDoctorsBySpeciality(speciality);
        return result;
    }
}