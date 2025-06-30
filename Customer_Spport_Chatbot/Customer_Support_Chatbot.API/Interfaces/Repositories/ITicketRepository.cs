using Customer_Support_Chatbot.Models;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        public Task<Agent?> GetAvailableAgentAsync();
        public Task<IEnumerable<Ticket>> GetTicketsHistoryAsync(Guid id, string role, string? keyword, string? timeRange);
        public Task EndTicketAsync(Guid ticketId);
        public Task<Ticket?> GetFullTicketAsync(Guid ticketId);
    }
}