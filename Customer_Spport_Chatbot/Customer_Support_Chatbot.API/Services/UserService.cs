using AutoMapper;
using Customer_Support_Chatbot.DTOs.User;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<string>> DeactivateAccountAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<string>.Fail("User not found.");
            }
            user.IsActive = false;
            user.IsDeactivated = true;
            _userRepository.Update(user);
            return ApiResponse<string>.Ok("User deactivated. Account will be deleted in 15 days.");
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
                return ApiResponse<string>.Fail("Email already registered.");
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
            return ApiResponse<string>.Ok("User registered successfully.");
        }
    }
}