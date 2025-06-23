using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Repositories
{
    public class AgentRepository : Repository<Agent>, IAgentRepository
    {
        private new readonly AppDbContext _context;
        public AgentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(Guid agentId)
        {
            if (agentId == Guid.Empty)
            {
                throw new ArgumentException("Agent ID cannot be empty.", nameof(agentId));
            }
            return await _context.Tickets
                .Where(t => t.AgentId == agentId)
                .Include(t => t.Messages)
                .Include(t => t.Attachments)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(Guid agentId, string status)
        {
            if (agentId == Guid.Empty)
            {
                throw new ArgumentException("Agent ID cannot be empty.", nameof(agentId));
            }
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status cannot be null or empty.", nameof(status));
            }

            var agent = await _context.Agents
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == agentId);

            if (agent != null)
            {
                agent.Status = status;
                agent.UpdatedAt = DateTime.UtcNow;
                if (agent.User != null)
                {
                    agent.User.IsActive = status.ToLower() == "available";
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"Agent with ID {agentId} not found.");
            }
        }
    }
}