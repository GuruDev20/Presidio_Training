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
        public TicketService(ITicketRepository ticketRepository,
                             IUserRepository userRepository,
                             IAgentRepository agentRepository,
                             IHubContext<ChatHub> hubContext)
        {
            _userRepository = userRepository;
            _agentRepository = agentRepository;
            _ticketRepository = ticketRepository;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<object>> AssignNewAgentAsync()
        {
            Console.WriteLine("Assigning new agent to the next priority ticket...");
            var ticket = await _ticketRepository.GetNextPriorityTicketAsync();
            if (ticket == null)
            {
                return ApiResponse<object>.Fail("No open tickets to assign.");
            }
            Console.WriteLine($"Found ticket {ticket.Id} with priority {ticket}");
            var newAgent = await _ticketRepository.GetAvailableAgentAsync();
            if (newAgent is null)
            {
                return ApiResponse<object>.Fail("No available agents at the moment. Please try again later.");
            }
            if (ticket.AgentId.HasValue)
            {
                var oldAgent = await _agentRepository.GetByIdAsync(ticket.AgentId.Value);
                if (oldAgent != null)
                {
                    oldAgent.Status = "Available";
                    var oldAgentUser = await _userRepository.GetByIdAsync(oldAgent.UserId);
                    if (oldAgentUser != null)
                    {
                        oldAgentUser.IsActive = true;
                        _userRepository.Update(oldAgentUser);
                    }
                    _agentRepository.Update(oldAgent);
                }
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
            // Create the ticket as open, but do not assign agent yet
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

            // Try to assign agent to the highest-priority open ticket (could be this one)
            await AssignNewAgentAsync();

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
                    }
                }
            }
            _ticketRepository.Update(ticket);
            await _ticketRepository.SaveChangesAsync();
            return ApiResponse<string>.Ok("Ticket has been successfully closed.");
        }

        public async Task<ApiResponse<object>> GetChatSessionAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetFullTicketAsync(ticketId);
            if (ticket == null)
            {
                return ApiResponse<object>.Fail("Ticket not found.");
            }
            var user = ticket.User;
            var agent = ticket.Agent;
            if (user == null || agent == null)
            {
                return ApiResponse<object>.Fail("User or Agent not found for this ticket.");
            }
            var messages = ticket.Messages?.OrderBy(m => m.SentAt).Select(m => new ChatMessageDto
            {
                Content = m.Content,
                SenderRole = m.SenderId == ticket.UserId ? "User" : "Agent",
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
        
        
        
    }
}