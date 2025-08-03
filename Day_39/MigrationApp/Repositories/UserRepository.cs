using MigrationApp.Contexts;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MigrationApp.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private new readonly AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(username));
            }
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);   
        }
    }
}