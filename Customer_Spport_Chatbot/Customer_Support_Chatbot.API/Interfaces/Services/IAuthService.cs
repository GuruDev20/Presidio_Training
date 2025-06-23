using Customer_Support_Chatbot.DTOs.Auth;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
        public Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string token);
        public Task<ApiResponse<string>> LogoutAsync(string refreshToken);
        public Task<ApiResponse<UserProfileDto>> GetProfileAsync(Guid userId);
    }
}