using System.Security.Claims;
using FirstAPI.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OAuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IRepository<string,User> _userRepository;
    public OAuthController(ITokenService tokenService,IRepository<string,User> userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    [HttpGet("login-google")]
    public IActionResult LoginWithGoogle([FromQuery] string role)
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse), "OAuth", new { role });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl! };
        return Challenge(properties, "Google");
    }


    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse([FromQuery] string role)
    {
        Console.WriteLine("Role received in GoogleResponse: " + role);
        var result = await HttpContext.AuthenticateAsync("Google");

        if (!result.Succeeded || result.Principal == null)
        {
            return Unauthorized(new { message = "Google authentication failed." });
        }

        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest(new { message = "Email claim not found in Google response." });
        }

        var user = await _userRepository.Get(email);
        if (user == null)
        {
            user = new User
            {
                Email = email,
                Role = role
            };
            await _userRepository.Add(user);
        }

        var token = await _tokenService.GenerateToken(user);
        return Ok(new { Token = token, Email = email, Role = user.Role });
    }

}