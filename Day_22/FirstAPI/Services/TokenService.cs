
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey;
    public TokenService(IConfiguration configuration)
    {
        _securityKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Keys:JwtTokenKey"]!));
    }
    public async Task<string> GenerateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.Email),
            new Claim(ClaimTypes.Role,user.Role)
        };
        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = credentials
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        if (token == null)
        {
            throw new Exception("Token generation failed");
        }
        return tokenHandler.WriteToken(token);
    }
}