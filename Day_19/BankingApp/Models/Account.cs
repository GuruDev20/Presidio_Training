using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Account
{
    public int AccountId { get; set; }
    [Required]
    public string AccountHolder { get; set; } = string.Empty;
    [Required]
    public string AccountType { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;
    public ICollection<Transactions> Transactions { get; set; } = new List<Transactions>();
}