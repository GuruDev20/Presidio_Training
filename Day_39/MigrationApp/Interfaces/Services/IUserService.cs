using MigrationApp.DTOs.User;
using MigrationApp.Wrappers;

namespace MigrationApp.Interfaces.Services
{
    public interface IUserService
    {
        public Task<ApiResponse<string>> RegisterUserAsync(RegisterRequestDto dto);
        public Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    }
}