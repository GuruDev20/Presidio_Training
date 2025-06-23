using Customer_Support_Chatbot.DTOs.User;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IUserService
    {
        public Task<ApiResponse<string>> RegisterUserAsync(RegisterRequestDto dto);
        public Task<ApiResponse<string>> DeactivateAccountAsync(Guid userId);
        public Task<ApiResponse<object>> GetUserTicketsAsync(Guid userId);
    }
}