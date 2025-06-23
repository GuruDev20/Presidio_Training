using Customer_Support_Chatbot.DTOs.Ticket;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAgentRepository _agentRepository;
        public TicketService(ITicketRepository ticketRepository,
                             IUserRepository userRepository,
                             IAgentRepository agentRepository)
        {
            _userRepository = userRepository;
            _agentRepository = agentRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<ApiResponse<object>> CreateTicketAsync(CreateTicketDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "CreateTicketDto cannot be null.");
            }
            var agent = await _ticketRepository.GetAvailableAgentAsync();
            if (agent == null)
            {
                return ApiResponse<object>.Fail("No available agents at the moment. Please try again later.");
            }
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = dto.Title,
                Description = dto.Description,
                UserId = dto.UserId,
                AgentId = agent.Id,
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

            var agentUser = await _userRepository.GetByIdAsync(agent.UserId);
            if (agentUser != null)
            {
                agentUser.IsActive = false;
                _userRepository.Update(agentUser);
            }
            agent.Status = "Busy";
            _agentRepository.Update(agent);
            var responseDto = new TicketResponseDto
            {
                TicketId = ticket.Id,
                AssignedAgentId = agent.Id,
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
            return ApiResponse<object>.Ok("Tickets history retrieved successfully.",data);
        }
    }
}