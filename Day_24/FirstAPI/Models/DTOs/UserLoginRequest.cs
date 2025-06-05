using System.ComponentModel.DataAnnotations;

public class UserLoginRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [MinLength(5, ErrorMessage = "Email must be at least 5 characters long.")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;
}