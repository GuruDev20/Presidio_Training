public class Account
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0.0m;
    public string? AccountStatus { get; set; } = null; 
    public ICollection<Transactions>? Transactions { get; set; }   
}