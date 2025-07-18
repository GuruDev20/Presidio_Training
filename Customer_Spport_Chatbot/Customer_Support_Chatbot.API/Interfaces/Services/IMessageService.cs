using Customer_Support_Chatbot.DTOs.Chat;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IMessageService
    {
        Task<List<UnifiedMessageDto>> GetMessagesAsync(Guid ticketId);
        Task<MessageDto> SendMessageAsync(SendMessageDto dto);
    }
}