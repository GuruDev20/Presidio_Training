using Customer_Support_Chatbot.DTOs.Agent;
using Customer_Support_Chatbot.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Support_Chatbot.Contexts
{
    [ApiController]
    [Route("api/v1/agents")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;
        public AgentController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [Authorize(Roles = "Agent,Admin")]
        [HttpGet("{agentId}/tickets")]
        public async Task<IActionResult> GetAssignedTickets(Guid agentId)
        {
            if (agentId == Guid.Empty)
            {
                return BadRequest("Agent ID cannot be empty.");
            }

            var response = await _agentService.GetAssignedTicketsAsync(agentId);
            if (response.Success)
            {
                return Ok(response.Data);
            }
            return NotFound(response.Message);
        }

        [Authorize(Roles = "Agent,Admin")]
        [HttpPut("{agentId}/status")]
        public async Task<IActionResult> UpdateStatus(Guid agentId, [FromBody] AgentStatusUpdateDto dto)
        {
            if (agentId == Guid.Empty)
            {
                return BadRequest("Agent ID cannot be empty.");
            }
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest("Invalid status update request.");
            }
            var response = await _agentService.UpdateStatusAsync(agentId, dto);
            if (response.Success)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}