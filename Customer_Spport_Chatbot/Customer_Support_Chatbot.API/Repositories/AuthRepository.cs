using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Repositories
{
    public class AuthRepository : Repository<User>, IAuthRepository
    {
        private new readonly AppDbContext _context;
        public AuthRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddDeactivationRequestAsync(DeactivationRequest request)
        {
            await _context.DeactivationRequests.AddAsync(request);
        }

        public async Task AddDeviceAsync(UserDevice device)
        {
            await _context.UserDevices.AddAsync(device);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<List<UserDevice>> GetActiveDevicesAsync(Guid userId)
        {
            return await _context.UserDevices
                .Where(d => d.UserId == userId && d.IsActive)
                .ToListAsync();
        }

        public async Task<UserDevice?> GetDeviceByIdAsync(Guid userId, string deviceId)
        {
            return await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));
            }
            return await _context.RefreshTokens.Include(rt=>rt.User).FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            return await _context.Users.Include(u=>u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email && !u.IsDeactivated);
        }

        public Task RemoveRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Remove(token);
            return Task.CompletedTask;
        }
    }
}