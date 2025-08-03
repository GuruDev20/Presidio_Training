
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public interface IAuthRepository : IRepository<User>
    {
        public Task<User?> GetUserByEmailAsync(string email);
    }
}
