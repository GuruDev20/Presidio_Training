public class AppointmentService : IAppointmentService
{
    private readonly IRepository<string, Appointment> _appointmentRepository;
    public AppointmentService(IRepository<string, Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Appointment> BookAppointmentAsync(Appointment appointment)
    {
        if (appointment == null)
        {
            throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null");
        }

        if (string.IsNullOrEmpty(appointment.AppointmentNumber))
        {
            throw new ArgumentException("Appointment number is required", nameof(appointment.AppointmentNumber));
        }
        if (string.IsNullOrEmpty(appointment.Status))
        {
            appointment.Status = "Booked";
        }
        return await _appointmentRepository.Add(appointment);
    }
    
    public async Task<Appointment> CancelAppointmentAsync(string appointmentNumber)
    {
        var appointment = await _appointmentRepository.Get(appointmentNumber);
        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found");
        }

        appointment.Status = "Cancelled";
        await _appointmentRepository.Delete(appointmentNumber);
        return appointment;
    }

    public Task<Appointment> GetAppointmentAsync(string appointmentNumber)
    {
        if (string.IsNullOrEmpty(appointmentNumber))
        {
            throw new ArgumentException("Appointment number is required", nameof(appointmentNumber));
        }

        return _appointmentRepository.Get(appointmentNumber);
    }
}