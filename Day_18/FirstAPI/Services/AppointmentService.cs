
public class AppointmentService : IAppointmentService
{
    private readonly IRepository<string, Appointment> _appointmentRepository;
    public AppointmentService(IRepository<string, Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public Task<Appointment> AddAppointmentAsync(Appointment appointment)
    {
        if (appointment == null)
        {
            throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
        }
        return _appointmentRepository.Add(appointment);
    }

    public Task<Appointment> DeleteAppointmentAsync(string AppointmentNumber)
    {
        if (string.IsNullOrWhiteSpace(AppointmentNumber))
        {
            throw new ArgumentException("Appointment number cannot be null or empty.", nameof(AppointmentNumber));
        }
        return _appointmentRepository.Delete(AppointmentNumber);
    }

    public Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        return _appointmentRepository.GetAll();
    }

    public Task<Appointment> GetAppointmentByIdAsync(string AppointmentNumber)
    {
        if (string.IsNullOrWhiteSpace(AppointmentNumber))
        {
            throw new ArgumentException("Appointment number cannot be null or empty.", nameof(AppointmentNumber));
        }
        return _appointmentRepository.Get(AppointmentNumber);
    }

    public Task<Appointment> UpdateAppointmentAsync(string AppointmentNumber, Appointment appointment)
    {
        if (string.IsNullOrWhiteSpace(AppointmentNumber))
        {
            throw new ArgumentException("Appointment number cannot be null or empty.", nameof(AppointmentNumber));
        }
        if (appointment == null)
        {
            throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
        }
        if (AppointmentNumber != appointment.AppointmentNumber)
        {
            throw new ArgumentException("Appointment number mismatch.", nameof(appointment));
        }
        return _appointmentRepository.Update(AppointmentNumber, appointment);
    }
}