using BCrypt.Net;
using MigrationApp.DTOs.Auth;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Wrappers;

namespace MigrationApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;
        public AuthService(IAuthRepository authRepository, IJwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(Guid userId)
        {
            var user = await _authRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<UserProfileDto>.Fail("User not found", 404);
            }
            return ApiResponse<UserProfileDto>.Ok("Profile fetched", new UserProfileDto
            {
                Id = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
            });
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid email or password", 401);
            }
            var accessToken = _jwtService.GenerateAccessToken(user);
            await _authRepository.SaveChangesAsync();
            return ApiResponse<AuthResponseDto>.Ok("Login successful", new AuthResponseDto
            {
                AccessToken = accessToken,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
            });
        }

        public Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            throw new NotImplementedException("Logout functionality is not implemented yet.");
        }
    }
}