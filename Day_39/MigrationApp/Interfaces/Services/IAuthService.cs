using MigrationApp.DTOs.Auth;
using MigrationApp.Wrappers;

namespace MigrationApp.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
        public Task<ApiResponse<string>> LogoutAsync(string refreshToken);
        public Task<ApiResponse<UserProfileDto>> GetProfileAsync(Guid userId);

    }
}