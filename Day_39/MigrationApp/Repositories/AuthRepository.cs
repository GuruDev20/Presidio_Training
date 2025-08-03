using Microsoft.EntityFrameworkCore;
using MigrationApp.Contexts;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Models;

namespace MigrationApp.Repositories
{
    public class AuthRepository : Repository<User>, IAuthRepository
    {
        private new readonly AppDbContext _context;
        public AuthRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}