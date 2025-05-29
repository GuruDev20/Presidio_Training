public interface ITransactionService
{
    public Task<TransactionDto> DepositAsync(int accountId, decimal amount);
    public Task<TransactionDto> WithdrawAsync(int accountId, decimal amount);
    public Task<decimal?> GetBalanceAsync(int accountId);
}