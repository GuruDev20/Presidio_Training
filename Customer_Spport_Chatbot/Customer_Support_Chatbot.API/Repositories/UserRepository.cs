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
    }
}