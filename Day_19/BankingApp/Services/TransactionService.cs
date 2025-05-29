
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public Task<TransactionDto> DepositAsync(int accountId, decimal amount)
    {
        if (accountId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(accountId), "Account ID must be greater than zero.");
        }
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be greater than zero.");
        }
        return _transactionRepository.DepositAsync(accountId, amount);
    }

    public Task<decimal?> GetBalanceAsync(int accountId)
    {
        if (accountId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(accountId), "Account ID must be greater than zero.");
        }
        return _transactionRepository.GetBalanceAsync(accountId);
    }

    public Task<TransactionDto> WithdrawAsync(int accountId, decimal amount)
    {
        if (accountId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(accountId), "Account ID must be greater than zero.");
        }
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal amount must be greater than zero.");
        }
        return _transactionRepository.WithdrawAsync(accountId, amount);
    }
}