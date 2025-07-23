using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Models.DTOs.Subscription;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<ApiResponse<Subscription>> CreateSubscriptionAsync(SubscriptionDto subscriptionDto)
        {
            try
            {
                // Validate required fields
                if (subscriptionDto.UserId == Guid.Empty)
                    throw new ArgumentException("UserId cannot be empty.");
                if (subscriptionDto.PaymentId == Guid.Empty)
                    throw new ArgumentException("PaymentId cannot be empty.");
                if (string.IsNullOrWhiteSpace(subscriptionDto.Tier))
                    throw new ArgumentException("Tier cannot be null or empty.");
                if (subscriptionDto.StartDate == default)
                    throw new ArgumentException("StartDate must be set.");
                if (subscriptionDto.EndDate == default)
                    throw new ArgumentException("EndDate must be set.");
                if (subscriptionDto.EndDate <= subscriptionDto.StartDate)
                    throw new ArgumentException("EndDate must be after StartDate.");

                var subscription = new Subscription
                {
                    UserId = subscriptionDto.UserId,
                    PaymentId = subscriptionDto.PaymentId,
                    Tier = subscriptionDto.Tier,
                    StartDate = subscriptionDto.StartDate,
                    EndDate = subscriptionDto.EndDate,
                    Currency = subscriptionDto.Currency,
                    Status = subscriptionDto.Status
                };

                var addedSubscription = await _subscriptionRepository.CreateSubscriptionAsync(subscription);

                if (addedSubscription == null)
                {
                    throw new Exception("Failed to add new subscription.");
                }

                return ApiResponse<Subscription>.Ok("Subscription created successfully.", addedSubscription);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<Subscription>.Fail($"Invalid subscription data: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<Subscription>.Fail("An error occurred while adding the subscription.", 500, ex);
            }
        }

       
    }
}