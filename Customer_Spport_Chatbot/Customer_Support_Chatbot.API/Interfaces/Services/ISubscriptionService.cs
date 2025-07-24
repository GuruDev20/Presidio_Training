
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IUserSubscriptionService
    {
        public Task<ApiResponse<UserSubscription>> CreateUserSubscriptionAsync(UserSubscriptionDto UserSubscriptionDto);
        public Task<ApiResponse<UserSubscription>> GetUserSubscriptionByIdAsync(Guid id);
        public Task<ApiResponse<IEnumerable<UserSubscription>>> GetAllUserSubscriptionsByUserIdAsync(Guid userId);
        public Task<ApiResponse<IEnumerable<UserSubscription>>> GetAllUserSubscriptionsAsync();
        public Task<ApiResponse<UserSubscription>> DeactivateUserSubscriptionAsync(Guid UserSubscriptionId);

    }
}