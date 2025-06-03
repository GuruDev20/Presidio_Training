public class AppointmentAddRequestDto
{
    public string AppointmentNumber{ get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; }= string.Empty;
}