using Customer_Support_Chatbot.DTOs.Admin;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IAdminService
    {
        public Task<ApiResponse<object>> AddAgentAsync(CreateAgentDto dto);
        public Task<ApiResponse<string>> DeleteAgentAsync(Guid agentId);
        public Task<ApiResponse<object>> UpdateAgentAsync(UpdateAgentDto dto);
        public Task<ApiResponse<object>> GetDashboardOverviewAsync();
        public Task<ApiResponse<object>> GetTicketGrowthAsync(string filter);
        Task<ApiResponse<List<DeactivationRequestDto>>> GetDeactivationRequestsAsync();
        Task<ApiResponse<List<AgentDto>>> GetAgentDetailsAsync();
        Task<ApiResponse<PagedTicketResponseDto>> GetTicketDetailsAsync(int page, int pageSize);
    }
}