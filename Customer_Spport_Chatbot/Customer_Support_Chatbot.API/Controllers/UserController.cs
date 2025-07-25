using System.Security.Claims;
using Customer_Support_Chatbot.DTOs.User;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Support_Chatbot.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid registration data.");
            }
            var response = await _userService.RegisterUserAsync(dto);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateAccountAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }
            var response = await _userService.DeactivateAccountAsync(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}/tickets")]
        public async Task<IActionResult> GetUserTicketsAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }
            var response = await _userService.GetUserTicketsAsync(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [Authorize(Roles = "User,Admin,Agent")]
        [HttpPost("profile/picture")]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfilePictureResponseDto file)
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<string>.Fail("Invalid token"));
            }
            var result = await _userService.UpdateProfilePictureAsync(userId, file);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Roles = "User,Admin,Agent")]
        [HttpPut("profile/name")]
        public async Task<IActionResult> UpdateProfileName([FromBody] UpdateNameRequestDto dto)
        {
            var userIdStr = User.FindFirstValue("sid");
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized(ApiResponse<string>.Fail("Invalid token"));
            }
            var result = await _userService.UpdateProfileNameAsync(userId, dto.FullName);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}