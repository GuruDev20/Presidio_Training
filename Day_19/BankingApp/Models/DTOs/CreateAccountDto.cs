using System.ComponentModel.DataAnnotations;

public class CreateAccountDto
{
    [Required]
    public string AccountHolder { get; set; } = string.Empty;
    [Required]
    public string AccountType { get; set; } = string.Empty;
    [Required]
    public decimal InitialBalance { get; set; }
}