using Customer_Support_Chatbot.DTOs.Admin;
using Customer_Support_Chatbot.DTOs.Ticket;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<ApiResponse<object>> AddAgentAsync(CreateAgentDto dto)
        {
            var hashedPwd = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = await _adminRepository.CreateAgentUserAsync(dto.Username, dto.Email, hashedPwd);
            var agent = await _adminRepository.CreateAgentAsync(user.Id);
            return ApiResponse<object>.Ok("Agent created successfully.", new
            {
                AgentId = agent.Id,
                Email = user.Email,
            });
        }

        public async Task<ApiResponse<string>> DeleteAgentAsync(Guid agentId)
        {
            if (agentId == Guid.Empty)
            {
                throw new ArgumentException("Agent ID cannot be empty.", nameof(agentId));
            }
            var result = await _adminRepository.DeleteAgentAsync(agentId);
            return result ? ApiResponse<string>.Ok("Agent has been successfully deleted.")
                : ApiResponse<string>.Fail("Agent not found or could not be deleted.", 404);
        }

        public async Task<ApiResponse<object>> GetDashboardOverviewAsync()
        {
            try
            {
                var overview = await _adminRepository.GetOverviewAsync();
                if (overview == null)
                {
                    return ApiResponse<object>.Fail("No overview data found.", 404);
                }
                return ApiResponse<object>.Ok("Dashboard overview retrieved successfully.", overview);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"An error occurred while retrieving the dashboard overview: {ex.Message}", 500);
            }
        }
        
        public async Task<ApiResponse<object>> GetTicketGrowthAsync(string filter)
        {
            try
            {
                var result = (List<TicketGrowthDto>)await _adminRepository.GetTicketGrowthAsync(filter);

                var labels = filter switch
                {
                    "last24hours" => result.Select(r => r.Date.ToString("HH:mm")).ToList(),
                    "last7days" => result.Select(r => r.Date.ToString("MMM dd")).ToList(),
                    "last30days" => result.Select(r => $"Week of {r.Date:MMM dd}").ToList(),
                    "last1year" => result.Select(r => r.Date.ToString("MMM yyyy")).ToList(),
                    _ => result.Select(r => r.Date.ToString("yyyy-MM-dd")).ToList()
                };

                var values = result.Select(r => r.Count).ToList();

                return ApiResponse<object>.Ok("Ticket growth data retrieved successfully.", new
                {
                    labels,
                    values
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Error retrieving ticket growth: {ex.Message}", 500);
            }
        }

    }
}