public interface IAccountRepository
{
    public Task<Account?> GetAccountNumberAsync(string accountNumber);
    public Task<Account?> GetLastAccountAsync();
    public Task AddAccountAsync(Account account);
    public Task UpdateAccountAsync(Account account);
}