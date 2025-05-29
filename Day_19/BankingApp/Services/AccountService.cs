
public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    public Task<Account> CreateAccountAsync(CreateAccountDto account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account), "Account cannot be null.");
        }
        return _accountRepository.CreateAccountAsync(account);
    }

    public Task<Account?> GetAccountByIdAsync(int accountId)
    {
        if (accountId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(accountId), "Account ID must be greater than zero.");
        }
        return _accountRepository.GetAccountByIdAsync(accountId);
    }
}