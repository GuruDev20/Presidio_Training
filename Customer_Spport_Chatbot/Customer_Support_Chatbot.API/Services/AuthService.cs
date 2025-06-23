using BCrypt.Net;
using Customer_Support_Chatbot.DTOs.Auth;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
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
                Id = user.Id,
                Email = user.Email,
                FullName = user.Username,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid email or password", 401);
            }
            if (user.IsDeactivated)
            {
                return ApiResponse<AuthResponseDto>.Fail("User is inactive", 403);
            }
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var newRefresh = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            await _authRepository.AddRefreshTokenAsync(newRefresh);
            await _authRepository.SaveChangesAsync();
            return ApiResponse<AuthResponseDto>.Ok("Login successful", new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Role = user.Role,
                UserId = user.Id,
                ExpiresMinutes = DateTime.UtcNow.AddMinutes(30)
            });
        }

        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            var existing = await _authRepository.GetRefreshTokenAsync(refreshToken);
            if (existing == null)
            {
                return ApiResponse<string>.Fail("Invalid refresh token", 404);
            }
            existing.IsRevoked = true;
            await _authRepository.SaveChangesAsync();
            return ApiResponse<string>.Ok("Logged out successfully");
        }

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string token)
        {
            var existing =await _authRepository.GetRefreshTokenAsync(token);
            if (existing == null || existing.ExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid or expired refresh token", 401);
            }
            var newAccessToken = _jwtService.GenerateAccessToken(existing.User!);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            existing.IsRevoked = true;
            var newRefresh = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = newRefreshToken,
                UserId = existing.UserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            await _authRepository.AddRefreshTokenAsync(newRefresh);
            await _authRepository.SaveChangesAsync();
            return ApiResponse<AuthResponseDto>.Ok("Token refreshed", new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Role = existing.User!.Role,
                UserId = existing.UserId,
                ExpiresMinutes = DateTime.UtcNow.AddMinutes(30)
            });
        }
    }
}