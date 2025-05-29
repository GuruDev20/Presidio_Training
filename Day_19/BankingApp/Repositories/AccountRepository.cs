
public class AccountRepository : IAccountRepository
{
    private readonly BankingContext _bankingContext;
    public AccountRepository(BankingContext bankingContext)
    {
        _bankingContext = bankingContext;
    }

    public async Task<Account> CreateAccountAsync(CreateAccountDto account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }
        var newAccount = new Account
        {
            AccountHolder = account.AccountHolder,
            AccountType = account.AccountType,
            Balance = account.InitialBalance,
        };
        _bankingContext.Accounts.Add(newAccount);
        await _bankingContext.SaveChangesAsync();
        return newAccount;
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId)
    {
        if (accountId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(accountId), "Account ID must be greater than zero.");
        }
        return await _bankingContext.Accounts.FindAsync(accountId);
    }
}