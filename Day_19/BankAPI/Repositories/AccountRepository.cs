
using Microsoft.EntityFrameworkCore;

public class StringResult
{
    public string? Value { get; set; }
}

public class AccountRepository : IAccountRepository
{

    private readonly BankingContext _context;
    public AccountRepository(BankingContext context)
    {
        _context = context;
    }

    public Task AddAccountAsync(Account account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        _context.Accounts.Add(account);
        return _context.SaveChangesAsync();
    }

    public async Task<Account?> GetAccountNumberAsync(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber))
        {
            throw new ArgumentException("Account number cannot be null or empty.", nameof(accountNumber));
        }
        return await _context.Accounts.Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<Account?> GetLastAccountAsync()
    {
        var result = await _context.Database
            .SqlQueryRaw<StringResult>("SELECT get_last_account_number() AS \"Value\"")
            .AsNoTracking()
            .FirstOrDefaultAsync();

        var lastAccountNumber = result?.Value;
        if (string.IsNullOrEmpty(lastAccountNumber))
        {
            return null;
        }
        return await GetAccountNumberAsync(lastAccountNumber);
    }


    public Task UpdateAccountAsync(Account account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }
        _context.Accounts.Update(account);
        return _context.SaveChangesAsync();
    }
}