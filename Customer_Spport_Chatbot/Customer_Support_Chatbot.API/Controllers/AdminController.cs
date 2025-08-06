using Customer_Support_Chatbot.DTOs.Admin;
using Customer_Support_Chatbot.Helpers;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Support_Chatbot.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly EmailHelper _emailHelper;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminController(IAdminService adminService, EmailHelper emailHelper, UserManager<IdentityUser> userManager)
        {
            _emailHelper = emailHelper;
            _userManager = userManager;
            _adminService = adminService;
        }

        [HttpPost("dashboard/create-agent")]
        public async Task<IActionResult> AddAgentAsync([FromBody] CreateAgentDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Agent data cannot be null.");
            }
            var response = await _adminService.AddAgentAsync(dto);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("dashboard/delete-agent/{agentId}")]
        public async Task<IActionResult> DeleteAgentAsync(Guid agentId)
        {
            Console.WriteLine($"Received request to delete agent with ID: {agentId}");
            if (agentId == Guid.Empty)
            {
                return BadRequest("Agent ID cannot be empty.");
            }
            var response = await _adminService.DeleteAgentAsync(agentId);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("dashboard/update-agent")]
        public async Task<IActionResult> UpdateAgentAsync([FromBody] UpdateAgentDto dto)
        {
            Console.WriteLine($"Received UpdateAgentDto: AgentId={dto.AgentId}, Username={dto.Username}");
            if (dto == null)
            {
                return BadRequest("Invalid agent data.");
            }
            Console.WriteLine($"Updating agent with ID: {dto.AgentId}, Username: {dto.Username}");
            var response = await _adminService.UpdateAgentAsync(dto);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("dashboard/overview")]
        public async Task<IActionResult> GetDashboardOverviewAsync()
        {
            var response = await _adminService.GetDashboardOverviewAsync();
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("dashboard/ticket-growth")]
        public async Task<IActionResult> GetTicketGrowthAsync([FromQuery] string filter)
        {
            var response = await _adminService.GetTicketGrowthAsync(filter);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("dashboard/deactivation-requests")]
        public async Task<IActionResult> GetDeactivationRequestsAsync()
        {
            var response = await _adminService.GetDeactivationRequestsAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("dashboard/agents")]
        public async Task<IActionResult> GetAgentDetailsAsync()
        {
            var response = await _adminService.GetAgentDetailsAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("dashboard/tickets")]
        public async Task<IActionResult> GetTicketDetailsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var response = await _adminService.GetTicketDetailsAsync(page, pageSize);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("dashboard/deactivation-request/{userId}")]
        public async Task<IActionResult> UpdateDeactivationRequestStatus(Guid userId, [FromBody] string status)
        {
            Console.WriteLine($"Received request to update deactivation request status for user ID: {userId}, Status: {status}");
            if (userId == Guid.Empty)
            {
                return BadRequest(ApiResponse<string>.Fail("User ID cannot be empty.", 400));
            }
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(ApiResponse<string>.Fail("Status cannot be empty.", 400));
            }

            try
            {
                var response = await _adminService.UpdateDeactivationRequestStatusAsync(userId, status);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail($"Error updating deactivation request status: {ex.Message}", 500));
            }
        }
    }
}