namespace BankingApp.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankingContext _bankingContext;

        public TransactionRepository(BankingContext bankingContext)
        {
            _bankingContext = bankingContext;
        }

        public async Task<TransactionDto> DepositAsync(int accountId, decimal amount)
        {
            if (accountId <= 0)
                throw new ArgumentOutOfRangeException(nameof(accountId), "Account ID must be greater than zero.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be greater than zero.");

            var account = await _bankingContext.Accounts.FindAsync(accountId);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID {accountId} not found.");

            account.Balance += amount;

            var transaction = new Transactions
            {
                AccountId = accountId,
                Amount = amount,
                TransactionType = "Deposit",
                TransactionDate = DateTime.UtcNow
            };

            await _bankingContext.Transactions.AddAsync(transaction);
            await _bankingContext.SaveChangesAsync();

            return new TransactionDto
            {
                AccountId = transaction.AccountId,
                Amount = transaction.Amount
            };
        }

        public async Task<TransactionDto> WithdrawAsync(int accountId, decimal amount)
        {
            if (accountId <= 0)
                throw new ArgumentOutOfRangeException(nameof(accountId), "Account ID must be greater than zero.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal amount must be greater than zero.");

            var account = await _bankingContext.Accounts.FindAsync(accountId);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID {accountId} not found.");

            if (account.Balance < amount)
                throw new InvalidOperationException("Insufficient funds for withdrawal.");

            account.Balance -= amount;

            var transaction = new Transactions
            {
                AccountId = accountId,
                Amount = amount,
                TransactionType = "Withdrawal",
                TransactionDate = DateTime.UtcNow
            };

            await _bankingContext.Transactions.AddAsync(transaction);
            await _bankingContext.SaveChangesAsync();

            return new TransactionDto
            {
                AccountId = transaction.AccountId,
                Amount = transaction.Amount
            };
        }

        public async Task<decimal?> GetBalanceAsync(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentOutOfRangeException(nameof(accountId), "Account ID must be greater than zero.");

            var account = await _bankingContext.Accounts.FindAsync(accountId);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID {accountId} not found.");

            return account.Balance;
        }
    }
}
