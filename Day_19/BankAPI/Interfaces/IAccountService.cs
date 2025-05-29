public interface IAccountService
{
    public Task<Account> CreateAccountAsync(CreateAccountRequest account);
    public Task<Account?> GetAccountByNumberAsync(string accountNumber);
    public Task<Account?> UpdateAccountAsync(UpdateAccountRequest account);
}