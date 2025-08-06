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

        public async Task NotifyAgent(string ticketId)
        {
            if (!Guid.TryParse(ticketId, out var parsedTicketId))
            {
                _logger.LogError("Invalid ticketId format: {TicketId}", ticketId);
                throw new HubException("Invalid ticket ID format.");
            }

            var ticket = await _context.Tickets
                .Include(t => t.Agent)
                .ThenInclude(a => a!.User)
                .FirstOrDefaultAsync(t => t.Id == parsedTicketId);
            if (ticket == null)
            {
                _logger.LogError("Ticket not found: {TicketId}", ticketId);
                throw new HubException("Ticket not found.");
            }

            if (ticket.AgentId.HasValue && ticket.Agent?.UserId != null)
            {
                _logger.LogInformation("Notifying agent {AgentUserId} for ticket {TicketId}: {Title}", ticket.Agent.UserId, ticketId, ticket.Name);
                await Clients.User(ticket.Agent.UserId.ToString()).SendAsync("ReceiveTicketAssignedNotification", new
                {
                    ticketId = ticketId,
                    title = ticket.Name
                });
            }
            else
            {
                _logger.LogWarning("No agent assigned to ticket {TicketId}", ticketId);
                throw new HubException("No agent assigned to ticket.");
            }
        }

        public async Task NotifySpecificAgent(string ticketId, string agentId)
        {
            if (!Guid.TryParse(ticketId, out var parsedTicketId) || !Guid.TryParse(agentId, out var parsedAgentId))
            {
                _logger.LogError("Invalid ticketId or agentId format: {TicketId}, {AgentId}", ticketId, agentId);
                throw new HubException("Invalid ticket ID or agent ID format.");
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == parsedTicketId);
            if (ticket == null)
            {
                _logger.LogError("Ticket not found: {TicketId}", ticketId);
                throw new HubException("Ticket not found.");
            }

            var agent = await _context.Agents
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == parsedAgentId);
            if (agent == null || Guid.Empty.Equals(agent.UserId))
            {
                _logger.LogError("Agent not found: {AgentId}", agentId);
                throw new HubException("Agent not found.");
            }

            _logger.LogInformation("Notifying specific agent {AgentUserId} for ticket {TicketId}: {Title}", agent.UserId, ticketId, ticket.Name);
            await Clients.User(agent.UserId.ToString()).SendAsync("ReceiveTicketAssignedNotification", new
            {
                ticketId = ticketId,
                title = ticket.Name
            });
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

        // public async Task NotifyAgentTicketAssigned(string agentUserId, string ticketId, string title)
        // {
        //     if (!Guid.TryParse(agentUserId, out var parsedAgentUserId) || !Guid.TryParse(ticketId, out var parsedTicketId))
        //     {
        //         _logger.LogError("Invalid agentUserId or ticketId format: {AgentUserId}, {TicketId}", agentUserId, ticketId);
        //         throw new HubException("Invalid agent user ID or ticket ID format.");
        //     }

        //     var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == parsedTicketId);
        //     if (!ticketExists)
        //     {
        //         _logger.LogError("Ticket not found: {TicketId}", ticketId);
        //         throw new HubException("Ticket not found.");
        //     }

        //     _logger.LogInformation("Notifying agent {AgentUserId} for ticket {TicketId}: {Title}", agentUserId, ticketId, title);
        //     await Clients.User(agentUserId).SendAsync("ReceiveTicketAssignedNotification", new
        //     {
        //         ticketId = ticketId,
        //         title = title
        //     });
        // }

        public async Task LeaveChat(string ticketId, string senderId, bool isAgent)
        {
            if (!Guid.TryParse(ticketId, out var parsedTicketId))
            {
                _logger.LogError("Invalid ticketId format: {TicketId}", ticketId);
                throw new HubException("Invalid ticket ID format.");
            }

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            _logger.LogInformation("Client {ConnectionId} (User: {UserId}) leaving group {TicketId}", Context.ConnectionId, userId, ticketId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticketId);
            await Clients.Group(ticketId).SendAsync("LeaveChat", new { ticketId, senderId, isAgent, text = $"{(isAgent ? "Agent" : "User")} has left the chat.", timestamp = DateTime.UtcNow.ToString("o") });
            if (_ticketAgents.TryGetValue(ticketId, out var agents) && agents.Remove(userId) && agents.Count == 0)
            {
                _ticketAgents.TryRemove(ticketId, out _);
            }
        }
    }
}