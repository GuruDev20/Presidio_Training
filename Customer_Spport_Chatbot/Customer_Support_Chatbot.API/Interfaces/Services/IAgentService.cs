using Customer_Support_Chatbot.DTOs.Agent;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IAgentService
    {
        public Task<ApiResponse<object>> GetAssignedTicketsAsync(Guid agentId);
        public Task<ApiResponse<string>> UpdateStatusAsync(Guid agentId, AgentStatusUpdateDto dto);
    }
}