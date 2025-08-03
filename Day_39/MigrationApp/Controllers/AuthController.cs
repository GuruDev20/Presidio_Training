using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MigrationApp.DTOs.Auth;
using MigrationApp.Interfaces.Services;
using MigrationApp.Wrappers;

namespace MigrationApp.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Invalid login data.");
            }
            var response = await _authService.LoginAsync(dto);
            if (response.Success)
            {
                return Ok(response);
            }
            return Unauthorized(response);
        }

        // [HttpGet("profile")]
        // public async Task<IActionResult> GetProfileAsync()
        // {
        //     var userIdStr = User.FindFirstValue("sid");
        //     if (!Guid.TryParse(userIdStr, out Guid userId))
        //     {
        //         return Unauthorized(ApiResponse<UserProfileDto>.Fail("Invalid token"));
        //     }
        //     var response = await _authService.GetProfileAsync(userId);
        //     if (response.Success)
        //     {
        //         return Ok(response);
        //     }
        //     return NotFound(response);
        // }

        // [HttpPost("logout")]
        // public async Task<IActionResult> LogoutAsync([FromBody] string refreshToken)
        // {
        //     if (string.IsNullOrEmpty(refreshToken))
        //     {
        //         return BadRequest("Refresh token is required.");
        //     }
        //     var response = await _authService.LogoutAsync(refreshToken);
        //     if (response.Success)
        //     {
        //         return Ok(response);
        //     }
        //     return BadRequest(response);
        // }
    }
}