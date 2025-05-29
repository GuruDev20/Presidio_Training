using FirstAPI.Contexts;
using FirstAPI.Misc;
using FirstAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

public class DoctorServiceWitTransaction : IDoctorService
{
    private readonly ClinicContext _context;
    private readonly DoctorMapper _doctorMapper;
    private readonly SpecialityMapper _specialityMapper;
    public DoctorServiceWitTransaction(ClinicContext context, DoctorMapper doctorMapper, SpecialityMapper specialityMapper)
    {
        _context = context;
        _doctorMapper = doctorMapper;
        _specialityMapper = specialityMapper;
    }

    public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctor)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var newDoctor = _doctorMapper.MapDoctorAddRequestDoctor(doctor);
            if (newDoctor == null)
            {
                throw new ArgumentNullException(nameof(newDoctor), "Doctor cannot be null");
            }
            await _context.Doctors.AddAsync(newDoctor);
            if (doctor.Specialities != null && doctor.Specialities.Any())
            {
                var existingSpecialities = await _context.Specialities.ToListAsync();
                foreach (var item in doctor.Specialities)
                {
                    var speciality = _context.Specialities.FirstOrDefault(s => s.Name.ToLower() == item.Name.ToLower());
                    if (speciality == null)
                    {
                        speciality = _specialityMapper.MapSpecialityAddRequestSpeciality(item);
                        await _context.AddAsync(speciality);
                        await _context.SaveChangesAsync();
                    }
                    var doctorSpeciality = _specialityMapper.MapDoctorSpeciality(newDoctor.Id, speciality.Id);
                    await _context.AddAsync(doctorSpeciality);
                }
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return newDoctor;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("An error occurred while adding the doctor.", ex);
        }
    }

    public Task<Doctor> GetDoctorByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<DoctorsBySpecialityResponseDTO>> GetDoctorsBySpecialityAsync(string specialityName)
    {
        var result = await _context.GetDoctorBySpeciality(specialityName);
        return result;
    }
}