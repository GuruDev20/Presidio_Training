using System.ComponentModel.DataAnnotations;

public class TransactionDto
{
    [Required]
    public int AccountId { get; set; }
    [Required]
    public decimal Amount { get; set; }
}