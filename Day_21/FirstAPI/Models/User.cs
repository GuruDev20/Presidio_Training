using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public byte[]? Password { get; set; }
    public byte[]? HashKey { get; set; }
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}