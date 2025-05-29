using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transactions
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int AccountId { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    [Required]
    public string TransactionType { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public Account Account { get; set; } = null!;
}