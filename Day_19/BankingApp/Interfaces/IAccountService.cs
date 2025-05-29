public interface IAccountService
{
    public Task<Account> CreateAccountAsync(CreateAccountDto account);
    public Task<Account?> GetAccountByIdAsync(int accountId);
}