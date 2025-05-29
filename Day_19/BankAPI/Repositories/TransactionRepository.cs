
using Microsoft.EntityFrameworkCore;

public class TransactionRepository : ITransactionRepository
{
    private readonly BankingContext _context;
    public TransactionRepository(BankingContext context)
    {
        _context = context;
    }

    public Task AddTransactionAsync(Transactions transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        _context.Transactions.Add(transaction);
        return _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Transactions>> GetTransactionsByAccountAsync(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber))
        {
            throw new ArgumentException("Account number cannot be null or empty.", nameof(accountNumber));
        }
        return await _context.Transactions
            .Where(t => t.AccountNumber == accountNumber)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }
}