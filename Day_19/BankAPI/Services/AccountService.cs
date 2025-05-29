
public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly AccountNumberGenerator _accountNumberGenerator;
    public AccountService(IAccountRepository accountRepository, AccountNumberGenerator accountNumberGenerator)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _accountNumberGenerator = accountNumberGenerator ?? throw new ArgumentNullException(nameof(accountNumberGenerator));
    }

    public async Task<Account> CreateAccountAsync(CreateAccountRequest account)
    {
        var lastAccount = await _accountRepository.GetLastAccountAsync();
        var lastAccountNumber = lastAccount?.AccountNumber;
        string newAccountNumber;
        if (string.IsNullOrEmpty(lastAccountNumber)) {
            newAccountNumber="ACC" + DateTime.Now.ToString("ddMMyy") + "AA0001";
        }
        else
        {
            newAccountNumber = _accountNumberGenerator.GenerateNextAccountNumber(lastAccountNumber);
        }
        var newAccount = new Account
        {
            AccountNumber = newAccountNumber,
            AccountHolderName = account.AccountHolderName,
            AccountType = account.AccountType,
            Balance = account.InitialBalance,
            AccountStatus = "Active",
        };
        await _accountRepository.AddAccountAsync(newAccount);
        return newAccount;
    }

    public Task<Account?> GetAccountByNumberAsync(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber))
        {
            throw new ArgumentException("Account number cannot be null or empty.", nameof(accountNumber));
        }
        return _accountRepository.GetAccountNumberAsync(accountNumber);
    }

    public async Task<Account?> UpdateAccountAsync(UpdateAccountRequest request)
    {
        var account = await _accountRepository.GetAccountNumberAsync(request.AccountNumber);
        if (account == null)
        {
            throw new KeyNotFoundException($"Account with number {request.AccountNumber} not found.");
        }
        if (!string.IsNullOrEmpty(request.AccountStatus))
        {
            account.AccountStatus = request.AccountStatus;
        }
        await _accountRepository.UpdateAccountAsync(account);
        return account;
    }
}