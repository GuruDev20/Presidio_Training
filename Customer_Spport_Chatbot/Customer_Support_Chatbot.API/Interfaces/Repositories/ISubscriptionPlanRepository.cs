using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface ISubscriptionPlanRepository
    {
        Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync();
        Task<SubscriptionPlan?> GetSubscriptionPlanByIdAsync(Guid id);
        Task<SubscriptionPlan> CreateSubscriptionPlan(SubscriptionPlanDto planDto);
        Task<SubscriptionPlan?> UpdateSubscriptionPlanAsync(Guid planId, SubscriptionPlanDto planDto);
        Task<bool> DeleteSubscriptionPlanAsync(Guid id);
    }
}