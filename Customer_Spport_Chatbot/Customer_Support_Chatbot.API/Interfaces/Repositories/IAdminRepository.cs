using Customer_Support_Chatbot.Models;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        public Task<User> CreateAgentUserAsync(string username, string email, string hashedPassword);
        public Task<Agent> CreateAgentAsync(Guid userId);
        public Task<bool> DeleteAgentAsync(Guid agentId);
        public Task<object> GetOverviewAsync();
        public Task<object> GetTicketGrowthAsync(string filter);
    }
}