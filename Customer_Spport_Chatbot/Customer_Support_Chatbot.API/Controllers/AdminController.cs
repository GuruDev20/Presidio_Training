using Customer_Support_Chatbot.DTOs.Admin;
using Customer_Support_Chatbot.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Support_Chatbot.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("agents")]
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

        [HttpDelete("agents/{agentId}")]
        public async Task<IActionResult> DeleteAgentAsync(Guid agentId)
        {
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
    }
}