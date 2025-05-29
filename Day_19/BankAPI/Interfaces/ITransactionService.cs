public interface ITransactionService
{
    public Task<bool> DepositAsync(string accountNumber, decimal amount);
    public Task<bool> WithdrawAsync(string accountNumber, decimal amount);
    public Task<IEnumerable<Transactions>> GetTransactionsByAccountAsync(string accountNumber);
    public Task<bool> TransferAsync(TransferRequest request);
}