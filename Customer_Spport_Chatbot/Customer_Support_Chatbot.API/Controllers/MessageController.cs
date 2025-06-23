using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.DTOs.Chat;
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
        private readonly AppDbContext _context;
        public MessageController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{ticketId}")]
        [Authorize]
        public async Task<IActionResult> GetMessages(Guid ticketId)
        {
            var messages = await _context.Messages
                .Where(m => m.TicketId == ticketId)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    TicketId = m.TicketId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SentAt = m.SentAt
                })
                .ToListAsync();

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

            var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == dto.TicketId);
            if (!ticketExists)
            {
                return NotFound("Ticket not found.");
            }

            var message = new Message
            {
                Id = Guid.NewGuid(),
                TicketId = dto.TicketId,
                SenderId = dto.SenderId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message.Id,
                message.TicketId,
                message.SenderId,
                message.Content,
                message.SentAt
            });
        }

    }
}