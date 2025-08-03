using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MigrationApp.DTOs.User;
using MigrationApp.Interfaces.Services;
using MigrationApp.Wrappers;

namespace MigrationApp.Controllers
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
    }
}