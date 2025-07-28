using Customer_Support_Chatbot.Models;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface IAuthRepository : IRepository<User>
    {
        public Task<User?> GetByIdWithSubscriptionsAsync(Guid userId);
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<RefreshToken?> GetRefreshTokenAsync(string token);
        public Task AddRefreshTokenAsync(RefreshToken refreshToken);
        public Task RemoveRefreshTokenAsync(RefreshToken token);
        public Task AddDeviceAsync(UserDevice device);
        public Task<List<UserDevice>> GetActiveDevicesAsync(Guid userId);
        public Task<UserDevice?> GetDeviceByIdAsync(Guid userId, string deviceId);
        public Task AddDeactivationRequestAsync(DeactivationRequest request);
    }
}