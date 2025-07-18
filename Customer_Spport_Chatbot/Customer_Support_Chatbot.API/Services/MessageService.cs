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

        public async Task<List<UnifiedMessageDto>> GetMessagesAsync(Guid ticketId)
        {
            Console.WriteLine($"Fetching messages and attachments for Ticket ID: {ticketId}");

            // Fetch ticket to get UserId and AgentId
            var ticket = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Agent)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            // Fetch messages
            var messages = await _context.Messages
                .Where(m => m.TicketId == ticketId)
                .Select(m => new UnifiedMessageDto
                {
                    Sender = m.SenderId == ticket.UserId ? "user" : "agent",
                    Text = m.Content,
                    FileUrl = null,
                    IsImage = false,
                    Timestamp = m.SentAt.ToString("o")
                })
                .ToListAsync();

            // Fetch attachments
            var attachments = await _context.FileAttachments
                .Where(a => a.TicketId == ticketId)
                .Select(a => new UnifiedMessageDto
                {
                    Sender = "user", // Adjust based on your logic if attachments can be sent by agents
                    Text = null,
                    FileUrl = $"/api/v1/files/{a.FileName}",
                    IsImage = a.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                              a.FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                              a.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                              a.FileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase),
                    Timestamp = a.UploadedAt.ToString("o")
                })
                .ToListAsync();

            // Combine and sort by timestamp
            var unifiedMessages = messages.Concat(attachments)
                .OrderBy(m => DateTime.Parse(m.Timestamp))
                .ToList();

            Console.WriteLine($"Fetched {unifiedMessages.Count} messages and attachments for Ticket ID: {ticketId}");
            return unifiedMessages;
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