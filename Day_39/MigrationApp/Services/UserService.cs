using MigrationApp.DTOs.User;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Models;
using MigrationApp.Wrappers;

namespace MigrationApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
            {
                return Task.FromResult(ApiResponse<string>.Fail("Old password is incorrect."));
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _userRepository.Update(user);
            _userRepository.SaveChangesAsync();
            return Task.FromResult(ApiResponse<string>.Ok("Password changed successfully."));
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
                UserId = Guid.NewGuid(),
                Email = dto.Email,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User",
            };
            await _userRepository.AddAsync(user);
            return ApiResponse<string>.Ok("User registered successfully.");
        }
    }
}