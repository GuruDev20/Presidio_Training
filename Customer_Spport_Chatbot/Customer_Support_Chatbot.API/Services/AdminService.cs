using Customer_Support_Chatbot.DTOs.Admin;
using Customer_Support_Chatbot.DTOs.Ticket;
using Customer_Support_Chatbot.Helpers;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;
using Microsoft.AspNetCore.Identity;

namespace Customer_Support_Chatbot.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly EmailHelper _emailHelper;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminService(IAdminRepository adminRepository, EmailHelper emailHelper, UserManager<IdentityUser> userManager)
        {
            _emailHelper = emailHelper;
            _userManager = userManager;
            _adminRepository = adminRepository;
        }

        public async Task<ApiResponse<object>> AddAgentAsync(CreateAgentDto dto)
        {
            try
            {
                var user = new IdentityUser
                {
                    UserName = dto.Username,
                    Email = dto.Email
                };
                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    return ApiResponse<object>.Fail($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}", 400);
                }

                await _userManager.AddToRoleAsync(user, "Agent");
                var agent= await _adminRepository.CreateAgentAsync(Guid.Parse(user.Id));
                var agentUser = await _adminRepository.CreateAgentUserAsync(user.UserName,user.Email,dto.Password);
                
                var emailBody = $"<h3>Welcome, {dto.Username}!</h3><p>Your agent account has been created.</p><p><strong>Credentials:</strong><br>Email: {dto.Email}<br>Password: {dto.Password}</p><p>Please log in at <a href='http://localhost:4200/login'>here</a> to access the system.</p>";
                var emailSent = _emailHelper.SendMail(dto.Email, "Your Agent Account Credentials", emailBody);

                if (!emailSent)
                {
                    Console.WriteLine("Email sending failed, but agent created successfully.");
                }

                return ApiResponse<object>.Ok("Agent created successfully.", new
                {
                    AgentId = agent.Id,
                    Email = user.Email,
                    Username = user.UserName
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Error creating agent: {ex.Message}", 500);
            }
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

        public async Task<ApiResponse<List<AgentDto>>> GetAgentDetailsAsync()
        {
            try
            {
                var agents = await _adminRepository.GetAgentDetailsAsync();
                return ApiResponse<List<AgentDto>>.Ok("Agent details retrieved successfully.", agents);
            }
            catch(Exception ex)
            {
                return ApiResponse<List<AgentDto>>.Fail($"An error occurred while retrieving agent details: {ex.Message}", 500);
            }
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

        public async Task<ApiResponse<List<DeactivationRequestDto>>> GetDeactivationRequestsAsync()
        {
            try
            {
                var requests = await _adminRepository.GetDeactivationRequestsAsync();
                return ApiResponse<List<DeactivationRequestDto>>.Ok("Deactivation requests retrieved successfully.", requests);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DeactivationRequestDto>>.Fail($"Error retrieving deactivation requests: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<PagedTicketResponseDto>> GetTicketDetailsAsync(int page, int pageSize)
        {
            try
            {
                var (tickets, totalCount) = await _adminRepository.GetTicketDetailsAsync(page, pageSize);
                var response = new PagedTicketResponseDto
                {
                    Tickets = tickets,
                    TotalCount = totalCount
                };
                return ApiResponse<PagedTicketResponseDto>.Ok("Ticket details retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedTicketResponseDto>.Fail($"Error retrieving ticket details: {ex.Message}", 500);
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

        public async Task<ApiResponse<object>> UpdateAgentAsync(UpdateAgentDto dto)
        {
            try
            {
                var agent = await _adminRepository.UpdateAgentAsync(dto.AgentId, dto.Username);
                if (agent == null)
                {
                    return ApiResponse<object>.Fail("Agent not found or could not be updated.", 404);
                }

                var user = await _userManager.FindByIdAsync(agent.UserId.ToString());
                if (user == null)
                {
                    return ApiResponse<object>.Fail("Associated user not found.", 404);
                }

                user.UserName = dto.Username;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return ApiResponse<object>.Fail($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}", 400);
                }

                var emailBody = $"<h3>Account Update Notification</h3><p>Your agent account details have been updated.</p><p><strong>Updated Details:</strong><br>Username: {dto.Username}<br>Email: {dto.Email}</p><p>Please log in at <a href='http://localhost:4200/login'>here</a> to access the system.</p>";
                var emailSent = _emailHelper.SendMail(dto.Email!, "Your Agent Account Updated", emailBody);

                if (!emailSent)
                {
                    Console.WriteLine("Email sending failed, but agent updated successfully.");
                }

                return ApiResponse<object>.Ok("Agent updated successfully.", new
                {
                    AgentId = agent.Id,
                    Username = agent.User!.Username,
                    Status = agent.Status
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Error updating agent: {ex.Message}", 500);
            }
        }
    }
}