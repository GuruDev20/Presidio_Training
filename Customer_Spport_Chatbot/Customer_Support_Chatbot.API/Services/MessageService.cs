using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.DTOs.Chat;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;
        public MessageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MessageDto>> GetMessagesAsync(Guid ticketId)
        {
            return await _context.Messages
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
        }

        public async Task<MessageDto> SendMessageAsync(SendMessageDto dto)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == dto.TicketId);
            if (ticket == null)
                throw new Exception("Ticket not found.");

            var message = new Message
            {
                Id = Guid.NewGuid(),
                TicketId = dto.TicketId,
                SenderId = dto.SenderId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            await _context.Messages.AddAsync(message);

            if (ticket.Status == "Open")
            {
                ticket.Status = "Pending";
                _context.Tickets.Update(ticket);
            }

            await _context.SaveChangesAsync();

            return new MessageDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                SenderId = message.SenderId,
                Content = message.Content,
                SentAt = message.SentAt
            };
        }
    }
}