using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface IUserSubscriptionRepository
    {
        Task<UserSubscription> CreateUserSubscriptionAsync(UserSubscriptionDto userSubscriptionDto);
        Task<UserSubscription?> GetUserSubscriptionByIdAsync(Guid id);
        Task<IEnumerable<UserSubscription>> GetAllUserSubscriptionsByUserIdAsync(Guid userId);
        Task<IEnumerable<UserSubscription>> GetAllUserSubscriptionsAsync();
        Task<UserSubscription?> DeactivateUserSubscriptionAsync(Guid userSubscriptionId);
        Task<UserSubscription?> ActivateUserSubscriptionAsync(Guid userSubscriptionId);
    }
}