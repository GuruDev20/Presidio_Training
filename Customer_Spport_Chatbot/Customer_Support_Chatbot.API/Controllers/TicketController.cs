using Customer_Support_Chatbot.API.DTOs.Chat;
using Customer_Support_Chatbot.DTOs.Ticket;
using Customer_Support_Chatbot.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Support_Chatbot.Contexts
{
    [ApiController]
    [Route("api/v1/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly AppDbContext _context;
        public TicketController(ITicketService ticketService, AppDbContext context)
        {
            _context = context;
            _ticketService = ticketService;
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Ticket data is required.");
            }

            var response = await _ticketService.CreateTicketAsync(dto);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response.Message);
        }

        [Authorize(Roles = "Agent")]
        [HttpPut("end/{ticketId}")]
        public async Task<IActionResult> EndTicket(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return BadRequest("Ticket ID is required.");
            }

            var response = await _ticketService.EndTicketAsync(ticketId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response.Message);
        }

        [Authorize(Roles = "User,Agent")]
        [HttpGet("history")]
        public async Task<IActionResult> GetTicketsHistory([FromQuery] TicketHistoryFilterDto filterDto)
        {
            if (filterDto == null)
            {
                return BadRequest("Filter data is required.");
            }

            var response = await _ticketService.GetTicketsHistoryAsync(filterDto);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response.Message);
        }

        [Authorize]
        [HttpGet("{ticketId}/chat-session")]
        public async Task<IActionResult> GetChatSession(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return BadRequest("Ticket ID is required.");
            }

            var response = await _ticketService.GetChatSessionAsync(ticketId);
            if (response.Success)
            {
                return Ok(response.Data);
            }
            return BadRequest(response.Message);
        }

        [Authorize]
        [HttpGet("agent/{agentId}/availability")]
        public async Task<IActionResult> CheckAgentAvailability(Guid agentId)
        {
            if (agentId == Guid.Empty)
            {
                return BadRequest("Agent ID is required.");
            }

            var response = await _ticketService.CheckAgentAvailabilityAsync(agentId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response.Message);
        }

        [Authorize]
        [HttpPost("assign-agent")]
        public async Task<IActionResult> AssignNewAgent()
        {
            var response = await _ticketService.AssignNewAgentAsync();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response.Message);
        }

        [Authorize]
        [HttpPost("leave-chat")]
        public async Task<IActionResult> LeaveChat([FromBody] LeaveChatDto dto)
        {
        if (dto == null || dto.TicketId == Guid.Empty || dto.UserId == Guid.Empty)
        {
            return BadRequest("Ticket ID and User ID are required.");
        }

        var response = await _ticketService.LeaveChatAsync(dto.TicketId, dto.UserId, dto.IsAgent);
        if (response.Success)
        {
            return Ok(response);
        }
        return BadRequest(response.Message);
        }
    }
}