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
        public TicketController(ITicketService ticketService)
        {
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
    }
}