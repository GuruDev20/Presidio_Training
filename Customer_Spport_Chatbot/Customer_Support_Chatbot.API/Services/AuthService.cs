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
            // Ensure Subscriptions are included
            var user = await _authRepository.GetByIdWithSubscriptionsAsync(userId);
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
                CreatedAt = user.CreatedAt,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Subscriptions = user.Subscriptions
            });
        }

        public async Task<ApiResponse<List<UserDeviceDto>>> GetUserDevicesAsync(Guid userId)
        {
            var devices = await _authRepository.GetActiveDevicesAsync(userId);
            var deviceDtos = devices.Select(d => new UserDeviceDto
            {
                DeviceId = d.DeviceId,
                DeviceType = d.DeviceType,
                OS = d.OS,
                Browser = d.Browser,
                LastLogin = d.LastLogin
            }).ToList();
            return ApiResponse<List<UserDeviceDto>>.Ok("Devices fetched", deviceDtos);
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request, string userAgent, string? ipAddress)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid email or password", 401);
            }
            if (user.IsDeactivated)
            {
                return ApiResponse<AuthResponseDto>.Fail("Your account has been deactivated, Try using by creating a new account", 403);
            }
            var activeDevices = await _authRepository.GetActiveDevicesAsync(user.Id);
            // Determine device limit by subscription plan
            int deviceLimit = 3; // Default for Basic
            if (user.Subscriptions != null && user.Subscriptions.Count > 0)
            {
                var now = DateTime.UtcNow;
                var activeSub = user.Subscriptions
                    .Where(s => s.Status == "Active" && s.StartDate <= now && s.EndDate >= now && s.Plan != null)
                    .OrderByDescending(s => s.Plan!.Priority)
                    .FirstOrDefault();
                if (activeSub != null && activeSub.Plan != null)
                {
                    switch (activeSub.Plan.Priority)
                    {
                        case 3:
                            deviceLimit = 20;
                            break;
                        case 2:
                            deviceLimit = 10;
                            break;
                        case 1:
                        default:
                            deviceLimit = 3;
                            break;
                    }
                }
            }
            if (activeDevices.Count >= deviceLimit)
            {
                return ApiResponse<AuthResponseDto>.Fail($"Maximum active devices limit reached for your plan ({deviceLimit}).", 403);
            }
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var deviceId = Guid.NewGuid().ToString();
            var newRefresh = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            var newDevice = new UserDevice
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                DeviceId = deviceId,
                DeviceType = "Unknown",
                OS = ParseOS(userAgent),
                Browser = ParseBrowser(userAgent),
                LastLogin = DateTime.UtcNow,
                IsActive = true,
            };
            await _authRepository.AddRefreshTokenAsync(newRefresh);
            await _authRepository.AddDeviceAsync(newDevice);
            await _authRepository.SaveChangesAsync();
            return ApiResponse<AuthResponseDto>.Ok("Login successful", new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Role = user.Role,
                UserId = user.Id,
                ExpiresMinutes = DateTime.UtcNow.AddMinutes(60),
                DeviceId = deviceId
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
            var existing = await _authRepository.GetRefreshTokenAsync(token);
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

        public async Task<ApiResponse<string>> RequestDeactivationAsync(Guid userId, string reason)
        {
            var user = await _authRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<string>.Fail("User not found", 404);
            }
            var deactivationRequest = new DeactivationRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Reason = reason,
                RequestedAt = DateTime.UtcNow,
                Status = "Pending"
            };
            await _authRepository.AddDeactivationRequestAsync(deactivationRequest);
            await _authRepository.SaveChangesAsync();
            return ApiResponse<string>.Ok("Deactivation request submitted");
        }

        public async Task<ApiResponse<string>> LogoutDeviceAsync(Guid userId, string deviceId)
        {
            var device = await _authRepository.GetDeviceByIdAsync(userId, deviceId);
            if (device == null)
            {
                return ApiResponse<string>.Fail("Device not found", 404);
            }
            device.IsActive = false;
            await _authRepository.SaveChangesAsync();
            return ApiResponse<string>.Ok("Device logged out successfully");
        }

        public async Task<ApiResponse<bool>> IsDeviceActiveAsync(Guid userId, string deviceId)
        {
            var device = await _authRepository.GetDeviceByIdAsync(userId, deviceId);
            if (device == null)
            {
                return ApiResponse<bool>.Fail("Device not found", 404);
            }
            return ApiResponse<bool>.Ok("Device status checked", device.IsActive);
        }
        
        private string ParseOS(string userAgent)
        {
            if (userAgent.Contains("Windows")) return "Windows";
            if (userAgent.Contains("Mac")) return "MacOS";
            if (userAgent.Contains("Linux")) return "Linux";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iOS")) return "iOS";
            return "Unknown";
        }

        private string ParseBrowser(string userAgent)
        {
            if (userAgent.Contains("Chrome")) return "Chrome";
            if (userAgent.Contains("Firefox")) return "Firefox";
            if (userAgent.Contains("Safari")) return "Safari";
            if (userAgent.Contains("Edge")) return "Edge";
            return "Unknown";
        }
    }
}