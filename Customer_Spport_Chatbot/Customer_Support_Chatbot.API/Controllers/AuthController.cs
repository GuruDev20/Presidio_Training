using System.Security.Claims;
using Customer_Support_Chatbot.DTOs.Auth;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Support_Chatbot.Controllers
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
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var result = await _authService.LoginAsync(dto, userAgent, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
        {
            var result = await _authService.LogoutAsync(dto.RefreshToken);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<string>.Fail("Invalid token"));
            }
            var result = await _authService.GetProfileAsync(userId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("deactivate")]
        public async Task<IActionResult> RequestDeactivation([FromBody] DeactivationRequestDto dto)
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<string>.Fail("Invalid token"));
            }
            var result = await _authService.RequestDeactivationAsync(userId, dto.Reason);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("devices")]
        public async Task<IActionResult> GetUserDevices()
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<string>.Fail("Invalid token"));
            }
            var result = await _authService.GetUserDevicesAsync(userId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("devices/logout")]
        public async Task<IActionResult> LogoutDevice([FromBody] DeviceLogoutRequestDto dto)
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<string>.Fail("Invalid token"));
            }
            var result = await _authService.LogoutDeviceAsync(userId, dto.DeviceId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("devices/{deviceId}/active")]
        public async Task<IActionResult> IsDeviceActive(string deviceId)
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<string>.Fail("Invalid token"));
            }
            var result = await _authService.IsDeviceActiveAsync(userId, deviceId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}