using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

public class AppointmentRepository : Repository<string, Appointment>
{
    public AppointmentRepository(ClinicContext clinicContext) : base(clinicContext) { }

    public override async Task<Appointment> Get(string key)
    {
        var appointment = await _clinicContext.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .SingleOrDefaultAsync(a => a.AppointmentNumber == key);

        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found");
        }

        return appointment;
    }

    public override async Task<IEnumerable<Appointment>> GetAll()
    {
        var appointments = await _clinicContext.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ToListAsync();

        if (appointments == null || !appointments.Any())
        {
            throw new KeyNotFoundException("No appointments found");
        }

        return appointments;
    }
}
