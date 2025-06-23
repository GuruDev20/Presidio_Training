using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.DTOs.Chat;
using Customer_Support_Chatbot.Models;
using Microsoft.AspNetCore.SignalR;

namespace Customer_Support_Chatbot.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task JoinRoom(string ticketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ticketId);
        }

        public async Task SendMessage(SendMessageDto dto)
        {
            var ticket=await _context.Tickets.FindAsync(dto.TicketId);
            if (ticket == null || ticket.Status == "Closed")
            {
                return;
            }
            var message = new Message
            {
                Id = Guid.NewGuid(),
                TicketId = dto.TicketId,
                SenderId = dto.SenderId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            await Clients.Group(dto.TicketId.ToString()).SendAsync("ReceiveMessage", new MessageDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                SenderId = message.SenderId,
                Content = message.Content,
                SentAt = message.SentAt
            });
        }

        public async Task EndChat(Guid ticketId, Guid AgentId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null || ticket.AgentId != AgentId)
            {
                await Clients.Caller.SendAsync("Error", "You are not authorized to end this chat.");
                return;
            }
            ticket.Status = "Closed";
            await _context.SaveChangesAsync();
            var agent = await _context.Agents.FindAsync(AgentId);
            if (agent != null)
            {
                agent.Status = "Available";
                agent.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            await Clients.Group(ticketId.ToString()).SendAsync("ReceiveMessage", new MessageDto
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                SenderId = Guid.Empty,
                Content = "Chat has been ended by the agent.",
                SentAt = DateTime.UtcNow
            });
        }
    }
}