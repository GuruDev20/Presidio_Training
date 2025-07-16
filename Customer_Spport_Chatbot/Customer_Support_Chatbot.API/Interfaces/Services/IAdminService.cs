using Customer_Support_Chatbot.DTOs.Admin;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IAdminService
    {
        public Task<ApiResponse<object>> AddAgentAsync(CreateAgentDto dto);
        public Task<ApiResponse<string>> DeleteAgentAsync(Guid agentId);
        public Task<ApiResponse<object>> GetDashboardOverviewAsync();
        public Task<ApiResponse<object>> GetTicketGrowthAsync(string filter);
    }
}