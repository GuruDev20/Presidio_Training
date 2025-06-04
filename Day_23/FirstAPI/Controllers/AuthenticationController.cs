using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthenticationController> _logger;
    public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResponse>> UserLogin(UserLoginRequest loginRequest)
    {
        try
        {
            var result = await _authenticationService.Login(loginRequest);
            if (result == null)
            {
                _logger.LogWarning("Login failed for user {Email}: User not found", loginRequest.Email);
                return NotFound("User not found");
            }
            _logger.LogInformation("User {Email} logged in successfully", loginRequest.Email);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during user login");
            return StatusCode(500, "Internal server error");
        }
    }
}