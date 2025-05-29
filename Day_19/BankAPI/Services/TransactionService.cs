
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
    }

    public async Task<bool> DepositAsync(string accountNumber, decimal amount)
    {
        var account = await _accountRepository.GetAccountNumberAsync(accountNumber);
        if (account == null) return false;
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));
        }
        account.Balance += amount;
        await _accountRepository.UpdateAccountAsync(account);
        await _transactionRepository.AddTransactionAsync(new Transactions
        {
            AccountNumber = accountNumber,
            TransactionType = "Deposit",
            Amount = amount,
            TransactionDate = DateTime.UtcNow
        });
        return true;
    }

    public Task<IEnumerable<Transactions>> GetTransactionsByAccountAsync(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber))
        {
            throw new ArgumentException("Account number cannot be null or empty.", nameof(accountNumber));
        }
        return _transactionRepository.GetTransactionsByAccountAsync(accountNumber);
    }

    public async Task<bool> TransferAsync(TransferRequest request)
    {
        var from = await _accountRepository.GetAccountNumberAsync(request.FromAccountNumber);
        var to = await _accountRepository.GetAccountNumberAsync(request.ToAccountNumber);
        if (from == null || to == null || from.Balance < request.Amount) return false;
        from.Balance -= request.Amount;
        to.Balance += request.Amount;
        await _accountRepository.UpdateAccountAsync(from);
        await _accountRepository.UpdateAccountAsync(to);
        await _transactionRepository.AddTransactionAsync(new Transactions
        {
            AccountNumber = request.FromAccountNumber,
            TransactionType = "Transfer Out",
            Amount = request.Amount,
            TransactionDate = DateTime.UtcNow
        });
        await _transactionRepository.AddTransactionAsync(new Transactions
        {
            AccountNumber = request.ToAccountNumber,
            TransactionType = "Transfer In",
            Amount = request.Amount,
            TransactionDate = DateTime.UtcNow
        });
        return true;
    }

    public async Task<bool> WithdrawAsync(string accountNumber, decimal amount)
    {
        var account = await _accountRepository.GetAccountNumberAsync(accountNumber);
        if (account == null) return false;
        if (amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));
        }
        if (account.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds for withdrawal.");
        }
        account.Balance -= amount;
        await _accountRepository.UpdateAccountAsync(account);
        await _transactionRepository.AddTransactionAsync(new Transactions
        {
            AccountNumber = accountNumber,
            TransactionType = "Withdrawal",
            Amount = amount,
            TransactionDate = DateTime.UtcNow
        });
        return true;
    }
}