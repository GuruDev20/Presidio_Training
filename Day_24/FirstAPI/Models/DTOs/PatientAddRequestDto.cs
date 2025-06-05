public class PatientAddRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
}