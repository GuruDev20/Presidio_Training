using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;
using Customer_Support_Chatbot.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface ISubscriptionPlanService
    {
        Task<ApiResponse<IEnumerable<SubscriptionPlan>>> GetAllSubscriptionPlansAsync();
        Task<ApiResponse<SubscriptionPlan>> GetSubscriptionPlanByIdAsync(Guid id);
        Task<ApiResponse<SubscriptionPlan>> CreateSubscriptionPlanAsync(SubscriptionPlanDto planDto);
        Task<ApiResponse<SubscriptionPlan>> UpdateSubscriptionPlanAsync(Guid planId, SubscriptionPlanDto planDto);
        Task<ApiResponse<bool>> DeleteSubscriptionPlanAsync(Guid id);
    }
}