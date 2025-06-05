using HRDocumentAPI.Contexts;
using HRDocumentAPI.DTOs;
using HRDocumentAPI.Interfaces;
using HRDocumentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HRDocumentAPI.Services
{
    public class UserService : IUserService
    {
        private readonly HRDocumentAPIContext _context;
        private readonly ITokenService _tokenService;
        private readonly IEncryptionService _encryptionService;
        public UserService(HRDocumentAPIContext context, ITokenService tokenService, IEncryptionService encryptionService)
        {
            _context = context;
            _tokenService = tokenService;
            _encryptionService = encryptionService;
        }
        public async Task<string> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null)
                {
                    throw new Exception("User not found.");
                }
                var encryptedPassword = await _encryptionService.EncryptData(new EncryptionModel { Data = dto.Password });
                if (user.Password!.SequenceEqual(encryptedPassword.EncryptedData!))
                {
                    throw new Exception("Invalid password.");
                }
                var userDto = new User
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    Status = user.Status
                };
                return await _tokenService.GenerateToken(userDto);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while logging in.", e);
            }
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var existingUser = await _context.Users.AnyAsync(u => u.Email == dto.Email);
                if (existingUser)
                {
                    throw new Exception("User with this email already exists.");
                }
                var encryptPassword = await _encryptionService.EncryptData(new EncryptionModel { Data = dto.Password });
                var newUser = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Password = encryptPassword.EncryptedData,
                    Role = dto.Role,
                    Status = dto.Status
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return await _tokenService.GenerateToken(newUser);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while registering the user.", e);
            }
        }
    }
}