public interface ITokenService
{
    public Task<string> GenerateToken(User user);
}