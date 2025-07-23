
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Subscription;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface ISubscriptionService
    {
        public Task<ApiResponse<Subscription>> CreateSubscriptionAsync(SubscriptionDto subscriptionDto);
        public Task<ApiResponse<Subscription>> GetSubscriptionByIdAsync(Guid id);
        public Task<ApiResponse<IEnumerable<Subscription>>> GetAllSubscriptionsByUserIdAsync(Guid userId);
        public Task<ApiResponse<IEnumerable<Subscription>>> GetAllSubscriptionsAsync();
        public Task<ApiResponse<Subscription>> DeactivateSubscriptionAsync(Guid subscriptionId);

    }
}