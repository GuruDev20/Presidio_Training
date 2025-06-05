public interface IAppointmentService
{
    public Task<Appointment> CancelAppointmentAsync(string appointmentNumber);
    public Task<Appointment> BookAppointmentAsync(Appointment appointment);
    public Task<Appointment> GetAppointmentAsync(string appointmentNumber);
}