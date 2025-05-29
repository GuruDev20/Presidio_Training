public class Transactions
{
    public int TransactionId { get; set; }
    public string AccountNumber { get; set; }= string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; } = 0.0m;
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public Account? Account { get; set; }
}