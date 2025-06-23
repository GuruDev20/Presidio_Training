using Customer_Support_Chatbot.Models;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface IAgentRepository : IRepository<Agent>
    {
        public Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(Guid agentId);
        public Task UpdateStatusAsync(Guid agentId, string status);
    }
}