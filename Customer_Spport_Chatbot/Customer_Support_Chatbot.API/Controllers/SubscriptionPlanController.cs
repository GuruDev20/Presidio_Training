using Microsoft.AspNetCore.Mvc;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;
using Customer_Support_Chatbot.Services;
using Customer_Support_Chatbot.Interfaces.Services;

namespace Customer_Support_Chatbot.Controllers
{
    [ApiController]
    [Route("api/v1/subscriptionPlans")]
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        public SubscriptionPlanController(ISubscriptionPlanService subscriptionPlanService)
        {
            _subscriptionPlanService = subscriptionPlanService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubscriptionPlan([FromBody] SubscriptionPlanDto dto)
        {
            var result = await _subscriptionPlanService.CreateSubscriptionPlanAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscriptionPlanById(Guid id)
        {
            var result = await _subscriptionPlanService.GetSubscriptionPlanByIdAsync(id);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubscriptionPlans()
        {
            var result = await _subscriptionPlanService.GetAllSubscriptionPlansAsync();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

    }
}
