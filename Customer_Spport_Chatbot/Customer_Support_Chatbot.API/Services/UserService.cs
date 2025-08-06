using AutoMapper;
using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.DTOs.User;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;
        public UserService(IUserRepository userRepository, IWebHostEnvironment environment, AppDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
            _environment = environment;
        }
        public Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return Task.FromResult(ApiResponse<string>.Fail("Old and new passwords cannot be empty."));
            }
            var user = _userRepository.GetByIdAsync(userId).Result;
            if (user == null)
            {
                return Task.FromResult(ApiResponse<string>.Fail("User not found."));
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                return Task.FromResult(ApiResponse<string>.Fail("Old password is incorrect."));
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _userRepository.Update(user);
            _userRepository.SaveChangesAsync();
            return Task.FromResult(ApiResponse<string>.Ok("Password changed successfully."));
        }

        public async Task<ApiResponse<string>> DeactivateAccountAsync(Guid userId,string reason)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(reason))
            {
                return ApiResponse<string>.Fail("Reason for deactivation cannot be empty.");
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<string>.Fail("User not found.");
            }
            var deactivationRequest = new DeactivationRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Reason = reason,
                RequestedAt = DateTime.UtcNow,
                Status = "Pending",
                User = user
            };
            await _userRepository.AddDeactivationRequestAsync(deactivationRequest);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            return ApiResponse<string>.Ok("User deactivated. Account will be deleted in 15 days.");
        }

        public async Task<ApiResponse<object>> GetAgentStatusAsync(Guid userId)
        {
            try
            {
                var agent = await _context.Agents
                    .Where(a => a.UserId == userId)
                    .Select(a => new { Status = a.Status })
                    .FirstOrDefaultAsync();

                if (agent == null)
                    return ApiResponse<object>.Fail("Agent not found.", 404);

                return ApiResponse<object>.Ok("Status retrieved successfully.", new { status = agent.Status });
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Error retrieving status: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<object>> GetUserTicketsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<object>.Fail("User not found.");
            }
            var tickets = await _userRepository.GetUserTicketsAsync(userId);
            if (tickets == null || !tickets.Any())
            {
                return ApiResponse<object>.Ok("No tickets found for this user.");
            }
            return ApiResponse<object>.Ok("Tickets retrieved successfully.", tickets);
        }

        public async Task<ApiResponse<string>> RegisterUserAsync(RegisterRequestDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                var deactivationRequests = await _context.DeactivationRequests
                    .Where(r => r.UserId == existingUser.Id && r.Status == "Pending")
                    .ToListAsync();

                if (deactivationRequests.Any())
                {
                    // Remove all pending deactivation requests
                    _context.DeactivationRequests.RemoveRange(deactivationRequests);
                    await _context.SaveChangesAsync();

                    // Update user status
                    existingUser.IsActive = true;
                    existingUser.IsDeactivated = false;
                    existingUser.DeactivationRequestedAt = null;
                    _userRepository.Update(existingUser);
                    await _userRepository.SaveChangesAsync();

                    return ApiResponse<string>.Ok("User reactivated successfully.");
                }
                else
                {
                    existingUser.IsActive = true;
                    existingUser.IsDeactivated = false;
                    existingUser.DeactivationRequestedAt = null;
                    _userRepository.Update(existingUser);
                    await _userRepository.SaveChangesAsync();

                    return ApiResponse<string>.Ok("User reactivated successfully.");
                }
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                return ApiResponse<string>.Fail("Passwords do not match.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                ProfilePictureUrl = dto.ProfilePictureUrl,
                IsActive = true,
                IsDeactivated = false,
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return ApiResponse<string>.Ok("User registered successfully.");
        }

        public async Task<ApiResponse<object>> UpdateAgentStatusAsync(Guid userId, string status)
        {
            try
            {
                if (!IsValidStatus(status))
                    return ApiResponse<object>.Fail("Invalid status. Allowed values: Available, Busy, Offline.", 400);

                var agent = await _context.Agents
                    .Where(a => a.UserId == userId)
                    .FirstOrDefaultAsync();

                if (agent == null)
                    return ApiResponse<object>.Fail("Agent not found.", 404);

                agent.Status = status;
                agent.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return ApiResponse<object>.Ok("Status updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Error updating status: {ex.Message}", 500);
            }
        }

        private bool IsValidStatus(string status)
        {
            return status == "Available" || status == "Busy" || status == "Offline";
        }

        public async Task<ApiResponse<string>> UpdateProfileNameAsync(Guid userId, string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return ApiResponse<string>.Fail("Name cannot be empty", 400);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<string>.Fail("User not found", 404);
            }

            user.Username = fullName;
            await _userRepository.SaveChangesAsync();
            return ApiResponse<string>.Ok("Name updated successfully");
        }

        public async Task<ApiResponse<UpdateProfilePictureResponseDto>> UpdateProfilePictureAsync(Guid userId, UpdateProfilePictureResponseDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<UpdateProfilePictureResponseDto>.Fail("User not found", 404);
            }
            user.ProfilePictureUrl = dto.ProfilePictureUrl;
            await _userRepository.SaveChangesAsync();
            return ApiResponse<UpdateProfilePictureResponseDto>.Ok("Profile picture updated", new UpdateProfilePictureResponseDto
            {
                ProfilePictureUrl = user.ProfilePictureUrl
            });
        }
    }
}