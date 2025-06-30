using Customer_Support_Chatbot.DTOs.Ticket;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface ITicketService
    {
        public Task<ApiResponse<object>> CreateTicketAsync(CreateTicketDto dto);
        public Task<ApiResponse<string>> EndTicketAsync(Guid ticketId);
        public Task<ApiResponse<object>> GetTicketsHistoryAsync(TicketHistoryFilterDto filterDto);
        public Task<ApiResponse<object>> GetChatSessionAsync(Guid ticketId);
    }
}