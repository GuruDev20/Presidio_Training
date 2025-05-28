public interface IAppointmentService
{
    public Task<Appointment> GetAppointmentByIdAsync(string AppointmentNumber);
    public Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    public Task<Appointment> AddAppointmentAsync(Appointment appointment);
    public Task<Appointment> UpdateAppointmentAsync(string AppointmentNumber, Appointment appointment);
    public Task<Appointment> DeleteAppointmentAsync(string AppointmentNumber);
}