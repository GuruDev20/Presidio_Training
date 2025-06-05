public class Patient
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public ICollection<Appointment>? Appointments { get; set; }
    public User? User { get; set; }
    
}