using Microsoft.AspNetCore.Mvc;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;
using Customer_Support_Chatbot.Services;
using Customer_Support_Chatbot.Interfaces.Services;

namespace Customer_Support_Chatbot.Controllers
{
    [ApiController]
    [Route("api/v1/subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly IUserSubscriptionService _subscriptionService;
        public SubscriptionController(IUserSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserSubscription([FromBody] UserSubscriptionDto dto)
        {
            var result = await _subscriptionService.CreateUserSubscriptionAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserSubscriptionById(Guid id)
        {
            var result = await _subscriptionService.GetUserSubscriptionByIdAsync(id);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllUserSubscriptionsByUserId(Guid userId)
        {
            var result = await _subscriptionService.GetAllUserSubscriptionsByUserIdAsync(userId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserSubscriptions()
        {
            var result = await _subscriptionService.GetAllUserSubscriptionsAsync();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUserSubscription(Guid id)
        {
            var result = await _subscriptionService.DeactivateUserSubscriptionAsync(id);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
