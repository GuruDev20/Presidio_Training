public class CreateAccountRequest
{
    public string AccountHolderName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; } = 0.0m;
}