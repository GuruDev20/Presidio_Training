
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User?> GetByEmailAsync(string email);
    }
}
