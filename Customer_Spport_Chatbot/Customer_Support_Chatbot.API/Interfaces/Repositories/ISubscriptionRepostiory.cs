using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Subscription;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
        Task<Subscription?> GetSubscriptionByIdAsync(Guid id);
        Task<IEnumerable<Subscription>> GetAllSubscriptionsByUserIdAsync(Guid userId);
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
        Task<Subscription?> DeactivateSubscriptionAsync(Guid subscriptionId);
    }
}