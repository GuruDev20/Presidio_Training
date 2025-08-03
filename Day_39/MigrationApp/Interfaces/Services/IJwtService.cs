
using System.Security.Claims;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
