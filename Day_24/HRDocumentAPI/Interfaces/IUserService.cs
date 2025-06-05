using HRDocumentAPI.DTOs;

namespace HRDocumentAPI.Interfaces
{
    public interface IUserService
    {
        public Task<string> RegisterAsync(RegisterDto dto);
        public Task<string> LoginAsync(LoginDto dto);
    }
}