public interface ITransactionRepository
{
    public Task AddTransactionAsync(Transactions transaction);
    public Task<IEnumerable<Transactions>> GetTransactionsByAccountAsync(string accountNumber);
}