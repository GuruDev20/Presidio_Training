using Customer_Support_Chatbot.DTOs.Auth;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request, string userAgent, string? ipAddress);
        public Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string token);
        public Task<ApiResponse<string>> LogoutAsync(string refreshToken);
        public Task<ApiResponse<UserProfileDto>> GetProfileAsync(Guid userId);
        public Task<ApiResponse<string>> RequestDeactivationAsync(Guid userId, string reason);
        public Task<ApiResponse<List<UserDeviceDto>>> GetUserDevicesAsync(Guid userId);
        public Task<ApiResponse<string>> LogoutDeviceAsync(Guid userId, string deviceId);
        public Task<ApiResponse<bool>> IsDeviceActiveAsync(Guid userId, string deviceId);
    }
}