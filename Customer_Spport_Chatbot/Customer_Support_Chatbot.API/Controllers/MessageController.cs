using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.DTOs.Chat;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Controllers
{
    [ApiController]
    [Route("api/v1/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{ticketId}")]
        [Authorize]
        public async Task<IActionResult> GetMessages(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return BadRequest("Ticket ID is required.");
            }
            var messages = await _messageService.GetMessagesAsync(ticketId);
            if (messages == null || !messages.Any())
            {
                return NotFound("No messages found for this ticket.");
            }
            return Ok(messages);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            if (dto.TicketId == Guid.Empty || dto.SenderId == Guid.Empty || string.IsNullOrWhiteSpace(dto.Content))
            {
                return BadRequest("Ticket ID, Sender ID, and message content are required.");
            }

            try
            {
                var message = await _messageService.SendMessageAsync(dto);
                if (message == null)
                {
                    return BadRequest("Failed to send message.");
                }
                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}