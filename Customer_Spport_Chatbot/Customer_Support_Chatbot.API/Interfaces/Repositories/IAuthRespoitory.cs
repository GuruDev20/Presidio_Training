using Customer_Support_Chatbot.Models;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface IAuthRepository : IRepository<User>
    {
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<RefreshToken?> GetRefreshTokenAsync(string token);
        public Task AddRefreshTokenAsync(RefreshToken refreshToken);
        public Task RemoveRefreshTokenAsync(RefreshToken token);
    }
}