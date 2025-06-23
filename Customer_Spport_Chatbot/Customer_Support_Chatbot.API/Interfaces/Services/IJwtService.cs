using System.Security.Claims;
using Customer_Support_Chatbot.Models;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}