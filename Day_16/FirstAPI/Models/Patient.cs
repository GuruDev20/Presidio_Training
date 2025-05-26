public class Patient
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfRegistration { get; set; }= DateTime.Now;
    public int DoctorId { get; set; }
    public string MedicalHistory { get; set; }= string.Empty;
        
}