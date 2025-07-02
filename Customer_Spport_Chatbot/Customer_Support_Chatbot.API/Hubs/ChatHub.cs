using System;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;
using Customer_Support_Chatbot.Contexts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Customer_Support_Chatbot.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChatHub> _logger;
        private static readonly ConcurrentDictionary<string, HashSet<string>> _ticketAgents = new();

        public ChatHub(AppDbContext context, ILogger<ChatHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            _logger.LogInformation("Client connected: {ConnectionId}, User: {UserId}", Context.ConnectionId, userId);
            await base.OnConnectedAsync();
        }

        public async Task JoinChat(string ticketId)
        {
            if (!Guid.TryParse(ticketId, out var parsedTicketId))
            {
                _logger.LogError("Invalid ticketId format: {TicketId}", ticketId);
                throw new HubException("Invalid ticket ID format.");
            }

            var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == parsedTicketId);
            if (!ticketExists)
            {
                _logger.LogError("Ticket not found: {TicketId}", ticketId);
                throw new HubException("Ticket not found.");
            }

            var isAgent = Context.User?.FindFirst(ClaimTypes.Role)?.Value == "Agent";
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            await Groups.AddToGroupAsync(Context.ConnectionId, ticketId);
            _logger.LogInformation("Client {ConnectionId} (User: {UserId}, Agent: {IsAgent}) joined group {TicketId}", Context.ConnectionId, userId, isAgent, ticketId);

            if (isAgent)
            {
                // Track agents for this ticket
                var agents = _ticketAgents.GetOrAdd(ticketId, _ => new HashSet<string>());
                if (!agents.Add(userId))
                {
                    _logger.LogWarning("Agent {UserId} already joined ticket {TicketId}, skipping AgentJoined event", userId, ticketId);
                    return;
                }

                _logger.LogInformation("Sending AgentJoined for ticket {TicketId} by user {UserId}", ticketId, userId);
                await Clients.Group(ticketId).SendAsync("AgentJoined", new
                {
                    sender = "agent",
                    text = "An agent has joined the chat.",
                    ticketId = ticketId,
                    timestamp = DateTime.UtcNow.ToString("o")
                });
            }
        }

        public async Task SendMessage(string ticketId, string senderId, string content)
        {
            if (!Guid.TryParse(ticketId, out var parsedTicketId) || !Guid.TryParse(senderId, out var parsedSenderId))
            {
                _logger.LogError("Invalid ticketId or senderId format: {TicketId}, {SenderId}", ticketId, senderId);
                throw new HubException("Invalid ticket ID or sender ID format.");
            }

            var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == parsedTicketId);
            if (!ticketExists)
            {
                _logger.LogError("Ticket not found: {TicketId}", ticketId);
                throw new HubException("Ticket not found.");
            }

            var senderType = Context.User?.FindFirst(ClaimTypes.Role)?.Value == "Agent" ? "agent" : "user";
            _logger.LogInformation("Sending message for ticket {TicketId} from {SenderType}: {Content}", ticketId, senderType, content);
            await Clients.Group(ticketId).SendAsync("ReceiveMessage", new
            {
                sender = senderType,
                text = content,
                ticketId = ticketId,
                timestamp = DateTime.UtcNow.ToString("o")
            });
        }

        public async Task SendFile(string ticketId, string senderId, string fileUrl, bool isImage)
        {
            if (!Guid.TryParse(ticketId, out var parsedTicketId) || !Guid.TryParse(senderId, out var parsedSenderId))
            {
                _logger.LogError("Invalid ticketId or senderId format: {TicketId}, {SenderId}", ticketId, senderId);
                throw new HubException("Invalid ticket ID or sender ID format.");
            }

            var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == parsedTicketId);
            if (!ticketExists)
            {
                _logger.LogError("Ticket not found: {TicketId}", ticketId);
                throw new HubException("Ticket not found.");
            }

            var senderType = Context.User?.FindFirst(ClaimTypes.Role)?.Value == "Agent" ? "agent" : "user";
            _logger.LogInformation("Sending file for ticket {TicketId} from {SenderType}: {FileUrl}", ticketId, senderType, fileUrl);
            await Clients.Group(ticketId).SendAsync("ReceiveFile", new
            {
                sender = senderType,
                fileUrl = fileUrl,
                isImage = isImage,
                ticketId = ticketId,
                timestamp = DateTime.UtcNow.ToString("o")
            });
        }

        public async Task EndChat(string ticketId)
        {
            if (!Guid.TryParse(ticketId, out var parsedTicketId))
            {
                _logger.LogError("Invalid ticketId format: {TicketId}", ticketId);
                throw new HubException("Invalid ticket ID format.");
            }

            var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == parsedTicketId);
            if (!ticketExists)
            {
                _logger.LogError("Ticket not found: {TicketId}", ticketId);
                throw new HubException("Ticket not found.");
            }

            _logger.LogInformation("Ending chat for ticket {TicketId}", ticketId);
            await Clients.Group(ticketId).SendAsync("ChatEnded", new
            {
                sender = "agent",
                text = "The support ticket has been closed.",
                ticketId = ticketId,
                timestamp = DateTime.UtcNow.ToString("o")
            });
            _ticketAgents.TryRemove(ticketId, out _); // Clear agents for this ticket
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticketId);
        }

        public async Task NotifyAgentTicketAssigned(string agentUserId, string ticketId, string title)
        {
            if (!Guid.TryParse(agentUserId, out var parsedAgentUserId) || !Guid.TryParse(ticketId, out var parsedTicketId))
            {
                _logger.LogError("Invalid agentUserId or ticketId format: {AgentUserId}, {TicketId}", agentUserId, ticketId);
                throw new HubException("Invalid agent user ID or ticket ID format.");
            }

            var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == parsedTicketId);
            if (!ticketExists)
            {
                _logger.LogError("Ticket not found: {TicketId}", ticketId);
                throw new HubException("Ticket not found.");
            }

            _logger.LogInformation("Notifying agent {AgentUserId} for ticket {TicketId}: {Title}", agentUserId, ticketId, title);
            await Clients.User(agentUserId).SendAsync("ReceiveTicketAssignedNotification", new
            {
                ticketId = ticketId,
                title = title
            });
        }

        public async Task LeaveChat(string ticketId)
        {
            if (!Guid.TryParse(ticketId, out var parsedTicketId))
            {
                _logger.LogError("Invalid ticketId format: {TicketId}", ticketId);
                throw new HubException("Invalid ticket ID format.");
            }

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            _logger.LogInformation("Client {ConnectionId} (User: {UserId}) leaving group {TicketId}", Context.ConnectionId, userId, ticketId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticketId);
            if (_ticketAgents.TryGetValue(ticketId, out var agents) && agents.Remove(userId) && agents.Count == 0)
            {
                _ticketAgents.TryRemove(ticketId, out _);
            }
        }
    }
}