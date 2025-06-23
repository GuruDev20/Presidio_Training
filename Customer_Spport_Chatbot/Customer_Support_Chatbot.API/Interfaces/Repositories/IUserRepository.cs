using Customer_Support_Chatbot.Models;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User?> GetByEmailAsync(string email);
        public Task<IEnumerable<Ticket>> GetUserTicketsAsync(Guid userId);
    }
}