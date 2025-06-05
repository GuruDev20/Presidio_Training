using HRDocumentAPI.Models;

namespace HRDocumentAPI.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}