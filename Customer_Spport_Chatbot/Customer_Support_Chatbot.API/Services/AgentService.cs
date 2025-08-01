using Customer_Support_Chatbot.DTOs.Agent;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
{
    public class AgentService : IAgentService
    {
        private readonly IAgentRepository _agentRepository;
        public AgentService(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository;
        }

        public async Task<ApiResponse<object>> GetAssignedTicketsAsync(Guid agentId)
        {
            if (agentId == Guid.Empty)
            {
                throw new ArgumentException("Agent ID cannot be empty.", nameof(agentId));
            }

            var tickets = await _agentRepository.GetAssignedTicketsAsync(agentId);
            if (tickets == null || !tickets.Any())
            {
                return ApiResponse<object>.Ok("No tickets found for the specified agent.");
            }
            return ApiResponse<object>.Ok("Tickets retrieved successfully.", tickets);
        }
    }
}