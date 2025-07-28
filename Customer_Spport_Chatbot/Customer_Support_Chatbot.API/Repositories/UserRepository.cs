using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private new readonly AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task AddDeactivationRequestAsync(DeactivationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Deactivation request cannot be null.");
            }
            _context.DeactivationRequests.Add(request);
            return _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            return await _context.Users.FirstOrDefaultAsync(u => u.Email== email);
        }

        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            return await _context.Tickets
                .Where(t => t.UserId == userId)
                .Include(t => t.Messages)
                .Include(t => t.Attachments)
                .ToListAsync();
        }

        public async Task<ICollection<User>> GetDeactivatedUsersAsync(DateTime threshold)
        {
            return await _context.Users
                .Where(u => u.IsDeactivated && u.DeactivationRequestedAt.HasValue && u.DeactivationRequestedAt <= threshold)
                .ToListAsync();
        }

        public async Task DeleteAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return;
                }
                var messages = await _context.Messages
                    .Where(m => m.SenderId == userId)
                    .ToListAsync();
                if (messages.Any())
                {
                    _context.Messages.RemoveRange(messages);
                }
                var deactivationRequests = await _context.DeactivationRequests
                    .Where(r => r.UserId == userId)
                    .ToListAsync();
                foreach (var request in deactivationRequests)
                {
                    request.Status = "Deleted";
                    _context.DeactivationRequests.Update(request);
                }
                var tickets = await _context.Tickets
                    .Where(t => t.UserId == userId)
                    .ToListAsync();
                if (tickets.Any())
                {
                    _context.Tickets.RemoveRange(tickets);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete user {userId}: {ex.Message}", ex);
            }
        }

        public Task UpdateDeactivationRequestStatusAsync(Guid userId, string status)
        {
            var request = _context.DeactivationRequests.FirstOrDefault(r => r.UserId == userId && r.Status == "Pending");
            if (request != null)
            {
                request.Status = status;
                _context.DeactivationRequests.Update(request);
                return _context.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }
    }
}