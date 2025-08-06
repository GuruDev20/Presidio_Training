using Customer_Support_Chatbot.API.DTOs.Chat;
using Customer_Support_Chatbot.DTOs.Chat;
using Customer_Support_Chatbot.DTOs.Ticket;
using Customer_Support_Chatbot.Hubs;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Wrappers;
using Microsoft.AspNetCore.SignalR;

namespace Customer_Support_Chatbot.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        public TicketService(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            IAgentRepository agentRepository,
            IHubContext<ChatHub> hubContext)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _agentRepository = agentRepository;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<object>> AssignNewAgentAsync(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return ApiResponse<object>.Fail("Invalid ticket ID.");
            }

            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return ApiResponse<object>.Fail("Ticket not found.");
            }

            // Check if ticket has an assigned agent and if they are available
            if (ticket.AgentId.HasValue)
            {
                var currentAgent = await _agentRepository.GetByIdAsync(ticket.AgentId.Value);
                if (currentAgent != null && currentAgent.Status == "Available")
                {
                    await _hubContext.Clients
                        .User(currentAgent.UserId.ToString())
                        .SendAsync("ReceiveTicketAssignedNotification", new
                        {
                            ticketId = ticket.Id,
                            title = ticket.Name
                        });
                    Console.WriteLine($"Current agent {currentAgent.Id} is available and notified.");
                    return ApiResponse<object>.Ok("Current agent is available and notified.", new { agentId = currentAgent.Id });
                }
            }

            // Assign a new agent
            var newAgent = await _ticketRepository.GetAvailableAgentAsync();
            if (newAgent == null)
            {
                return ApiResponse<object>.Fail("No available agents at the moment. Please try again later.");
            }

            ticket.AgentId = newAgent.Id;
            newAgent.Status = "Busy";
            var newAgentUser = await _userRepository.GetByIdAsync(newAgent.UserId);
            if (newAgentUser != null)
            {
                newAgentUser.IsActive = false;
                _userRepository.Update(newAgentUser);
            }
            _agentRepository.Update(newAgent);
            _ticketRepository.Update(ticket);
            await _ticketRepository.SaveChangesAsync();

            await _hubContext.Clients
                .User(newAgent.UserId.ToString())
                .SendAsync("ReceiveTicketAssignedNotification", new
                {
                    ticketId = ticket.Id,
                    title = ticket.Name
                });

            return ApiResponse<object>.Ok("New agent assigned successfully.", new { agentId = newAgent.Id });
        }

        public async Task<ApiResponse<object>> CheckAgentAvailabilityAsync(Guid agentId)
        {
            if (agentId == Guid.Empty)
            {
                return ApiResponse<object>.Fail("Agent ID cannot be empty.");
            }

            var agent = await _agentRepository.GetByIdAsync(agentId);
            if (agent == null)
            {
                return ApiResponse<object>.Fail("Agent not found.");
            }
            var isAvailable = agent.Status == "Available";
            return ApiResponse<object>.Ok("Agent availability checked.", new { isAvailable });
        }

        public async Task<ApiResponse<object>> CreateTicketAsync(CreateTicketDto dto)
{
    if (dto == null)
    {
        throw new ArgumentNullException(nameof(dto), "CreateTicketDto cannot be null.");
    }

    var ticket = new Ticket
    {
        Id = Guid.NewGuid(),
        Name = dto.Title,
        Description = dto.Description,
        UserId = dto.UserId,
        Status = "Open",
        CreatedAt = DateTime.UtcNow
    };
    await _ticketRepository.AddAsync(ticket);
    var user = await _userRepository.GetByIdAsync(dto.UserId);
    if (user != null)
    {
        user.IsActive = false;
        _userRepository.Update(user);
    }

    var newAgent = await _ticketRepository.GetAvailableAgentAsync();
    if (newAgent == null)
    {
        Console.WriteLine($"[TicketService] No available agents for ticket {ticket.Id}");
        return ApiResponse<object>.Fail("No available agents at the moment. Please try again later.");
    }

    ticket.AgentId = newAgent.Id;
    newAgent.Status = "Busy";
    var newAgentUser = await _userRepository.GetByIdAsync(newAgent.UserId);
    if (newAgentUser != null)
    {
        newAgentUser.IsActive = false;
        _userRepository.Update(newAgentUser);
        Console.WriteLine($"[TicketService] Agent {newAgent.Id} (UserId: {newAgentUser.Id}) assigned to ticket {ticket.Id}, set to Busy and IsActive=false");
    }
    else
    {
        Console.WriteLine($"[TicketService] Agent User not found for AgentId {newAgent.Id}");
    }
    _agentRepository.Update(newAgent);
    _ticketRepository.Update(ticket);
    await _ticketRepository.SaveChangesAsync();

    // Send notification to agent
    Console.WriteLine($"[TicketService] Sending ReceiveTicketAssignedNotification to UserId: {newAgent.UserId} for ticket {ticket.Id}");
    await _hubContext.Clients
        .User(newAgent.UserId.ToString())
        .SendAsync("ReceiveTicketAssignedNotification", new
        {
            ticketId = ticket.Id,
            title = ticket.Name
        });

    var responseDto = new TicketResponseDto
    {
        TicketId = ticket.Id,
        AssignedAgentId = ticket.AgentId ?? Guid.Empty,
        Title = ticket.Name!,
        Description = ticket.Description
    };
    return ApiResponse<object>.Ok("Ticket created successfully.", responseDto);
}

        public async Task<ApiResponse<string>> EndTicketAsync(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                throw new ArgumentException("Ticket ID cannot be empty.", nameof(ticketId));
            }
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return ApiResponse<string>.Fail("Ticket not found.");
            }

            ticket.Status = "Closed";
            ticket.ClosedAt = DateTime.UtcNow;
            var user = await _userRepository.GetByIdAsync(ticket.UserId);
            if (user != null)
            {
                user.IsActive = true;
                _userRepository.Update(user);
            }
            if (ticket.AgentId.HasValue)
            {
                var agent = await _agentRepository.GetByIdAsync(ticket.AgentId.Value);
                if (agent != null)
                {
                    agent.Status = "Available";
                    var agentUser = await _userRepository.GetByIdAsync(agent.UserId);
                    if (agentUser != null)
                    {
                        agentUser.IsActive = true;
                        _userRepository.Update(agentUser);
                    }
                    _agentRepository.Update(agent);
                }
            }
            _ticketRepository.Update(ticket);
            await _ticketRepository.SaveChangesAsync();
            return ApiResponse<string>.Ok("Ticket has been successfully closed.");
        }

        public async Task<ApiResponse<object>> GetChatSessionAsync(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                return ApiResponse<object>.Fail("Ticket ID cannot be empty.");
            }

            var ticket = await _ticketRepository.GetFullTicketAsync(ticketId);
            if (ticket == null)
            {
                return ApiResponse<object>.Fail("Ticket not found.");
            }
            var user = ticket.User;
            var agent = ticket.Agent;
            var messages = ticket.Messages?.OrderBy(m => m.SentAt).Select(m => new ChatMessageDto
            {
                Content = m.Content,
                SenderRole = m.SenderId == ticket.UserId ? "user" : "agent",
                SentAt = m.SentAt
            }).ToList() ?? new List<ChatMessageDto>();
            var attachments = ticket.Attachments?.Select(a => new ChatAttachmentDto
            {
                FileName = a.FileName,
                Url = $"/api/v1/files/{a.FileName}",
                SendAt = a.UploadedAt
            }).ToList() ?? new List<ChatAttachmentDto>();

            var chatDto = new ChatSessionDto
            {
                TicketId = ticket.Id,
                Title = ticket.Name ?? "Untitled Ticket",
                Status = ticket.Status,
                UserName = user?.Username ?? "Unknown User",
                AgentName = agent?.User?.Username ?? "Unassigned",
                Messages = messages,
                Attachments = attachments,
            };
            return ApiResponse<object>.Ok("Chat session retrieved successfully.", chatDto);
        }

        public async Task<ApiResponse<object>> GetTicketsHistoryAsync(TicketHistoryFilterDto filterDto)
        {
            if (filterDto == null)
            {
                throw new ArgumentNullException(nameof(filterDto), "TicketHistoryFilterDto cannot be null.");
            }
            var data = await _ticketRepository.GetTicketsHistoryAsync(
                filterDto.UserOrAgentId,
                filterDto.Role,
                filterDto.SearchKeyword,
                filterDto.TimeRange
            );
            return ApiResponse<object>.Ok("Tickets history retrieved successfully.", data);
        }

        public async Task<ApiResponse<string>> LeaveChatAsync(Guid ticketId, Guid userId, bool isAgent)
{
    Console.WriteLine($"LeaveChat called with TicketId: {ticketId}, UserId: {userId}, IsAgent: {isAgent}");
    if (ticketId == Guid.Empty || userId == Guid.Empty)
    {
        return ApiResponse<string>.Fail("Ticket ID and User ID cannot be empty.");
    }

    var ticket = await _ticketRepository.GetByIdAsync(ticketId);
    if (ticket == null)
    {
        return ApiResponse<string>.Fail("Ticket not found.");
    }

    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null)
    {
        return ApiResponse<string>.Fail("User not found.");
    }

    if (isAgent)
    {
        // Agent leaving the chat
        await _hubContext.Clients
            .User(ticket.UserId.ToString())
            .SendAsync("UserLeftChat", new
            {
                ticketId,
                text = "The agent has left the chat.",
                sender = "bot",
                timestamp = DateTime.UtcNow.ToString("o")
            });
        user.IsActive = true;
        _userRepository.Update(user);
        // Update agent status
        var agent = await _agentRepository.GetByIdAsync(userId);
        if (agent != null)
        {
            agent.Status = "Available";
            var agentUser = await _userRepository.GetByIdAsync(agent.UserId);
            if (agentUser != null)
            {
                agentUser.IsActive = true;
                _userRepository.Update(agentUser);
                Console.WriteLine($"[TicketService] Agent {agent.Id} (UserId: {agentUser.Id}) set to Available and IsActive=true");
            }
            _agentRepository.Update(agent);
        }
        else
        {
            Console.WriteLine($"[TicketService] Agent not found for UserId: {userId}");
        }
    }
    else
    {
        // User leaving the chat
        if (ticket.UserId != userId)
        {
            return ApiResponse<string>.Fail("User is not associated with this ticket.");
        }

        // Notify agent that user has left
        if (ticket.AgentId.HasValue)
        {
            var agent = await _agentRepository.GetByIdAsync(ticket.AgentId.Value);
            if (agent != null)
            {
                var agentUser = await _userRepository.GetByIdAsync(agent.UserId);
                if (agentUser != null)
                {
                    await _hubContext.Clients
                        .User(agentUser.Id.ToString())
                        .SendAsync("UserLeftChat", new
                        {
                            ticketId,
                            text = "The user has left the chat.",
                            sender = "bot",
                            timestamp = DateTime.UtcNow.ToString("o")
                        });
                    agent.Status = "Available";
                    agentUser.IsActive = true;
                    _agentRepository.Update(agent);
                    _userRepository.Update(agentUser);
                    Console.WriteLine($"[TicketService] Agent {agent.Id} (UserId: {agentUser.Id}) set to Available and IsActive=true due to user leaving");
                }
                else
                {
                    Console.WriteLine($"[TicketService] Agent User not found for AgentId: {agent.Id}");
                }
            }
            else
            {
                Console.WriteLine($"[TicketService] Agent not found for AgentId: {ticket.AgentId.Value}");
            }
        }

        // Update user status
        user.IsActive = true;
        _userRepository.Update(user);
        Console.WriteLine($"[TicketService] User {userId} set to IsActive=true");
    }

    await _ticketRepository.SaveChangesAsync();
    return ApiResponse<string>.Ok("Chat session left successfully.");
}
    }
}