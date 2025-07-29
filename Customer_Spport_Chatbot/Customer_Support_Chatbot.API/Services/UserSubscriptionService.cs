using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUserSubscriptionRepository _UserSubscriptionRepository;
        private readonly IPaymentRepository _PaymentRepository;
        private readonly IOrderRepository _OrderRepository;

        public UserSubscriptionService(IUserSubscriptionRepository UserSubscriptionRepository)
        {
            _UserSubscriptionRepository = UserSubscriptionRepository;
        }

        public async Task<ApiResponse<UserSubscription>> CreateUserSubscriptionAsync(UserSubscriptionDto UserSubscriptionDto)
        {
            try
            {
                // Validate required fields
                if (UserSubscriptionDto.UserId == Guid.Empty)
                    throw new ArgumentException("UserId cannot be empty.");
                if (UserSubscriptionDto.PlanId == Guid.Empty)
                    throw new ArgumentException("PlanId cannot be empty.");
                if (UserSubscriptionDto.PaymentId == Guid.Empty)
                    throw new ArgumentException("PaymentId cannot be empty.");
                if (UserSubscriptionDto.StartDate == default)
                    throw new ArgumentException("StartDate must be set.");
                if (UserSubscriptionDto.EndDate == default)
                    throw new ArgumentException("EndDate must be set.");
                if (UserSubscriptionDto.EndDate <= UserSubscriptionDto.StartDate)
                    throw new ArgumentException("EndDate must be after StartDate.");

                var addedUserSubscription = await _UserSubscriptionRepository.CreateUserSubscriptionAsync(UserSubscriptionDto);

                if (addedUserSubscription == null)
                {
                    throw new Exception("Failed to add new UserSubscription.");
                }

                return ApiResponse<UserSubscription>.Ok("UserSubscription created successfully.", addedUserSubscription);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<UserSubscription>.Fail($"Invalid UserSubscription data: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserSubscription>.Fail("An error occurred while adding the UserSubscription.", 500, ex);
            }
        }

        public async Task<ApiResponse<UserSubscription>> GetUserSubscriptionByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("UserSubscription ID cannot be empty.", nameof(id));
                }

                var userSubscription = await _UserSubscriptionRepository.GetUserSubscriptionByIdAsync(id);
                if (userSubscription == null)
                {
                    return ApiResponse<UserSubscription>.Fail("UserSubscription not found.", 404);
                }

                return ApiResponse<UserSubscription>.Ok("UserSubscription retrieved successfully.", userSubscription);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<UserSubscription>.Fail($"Invalid UserSubscription ID: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserSubscription>.Fail("An error occurred while retrieving the UserSubscription.", 500, ex);
            }
        }

        public async Task<ApiResponse<IEnumerable<UserSubscription>>> GetAllUserSubscriptionsByUserIdAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("User ID cannot be empty.", nameof(userId));
                }

                var userSubscriptions = await _UserSubscriptionRepository.GetAllUserSubscriptionsByUserIdAsync(userId);
                return ApiResponse<IEnumerable<UserSubscription>>.Ok("UserSubscriptions retrieved successfully.", userSubscriptions);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<IEnumerable<UserSubscription>>.Fail($"Invalid User ID: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<UserSubscription>>.Fail("An error occurred while retrieving UserSubscriptions.", 500, ex);
            }
        }

        public async Task<ApiResponse<IEnumerable<UserSubscription>>> GetAllUserSubscriptionsAsync()
        {
            try
            {
                var userSubscriptions = await _UserSubscriptionRepository.GetAllUserSubscriptionsAsync();
                return ApiResponse<IEnumerable<UserSubscription>>.Ok("UserSubscriptions retrieved successfully.", userSubscriptions);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<UserSubscription>>.Fail("An error occurred while retrieving UserSubscriptions.", 500, ex);
            }
        }

        public async Task<ApiResponse<UserSubscription>> DeactivateUserSubscriptionAsync(Guid userSubscriptionId)
        {
            try
            {
                if (userSubscriptionId == Guid.Empty)
                {
                    throw new ArgumentException("UserSubscription ID cannot be empty.", nameof(userSubscriptionId));
                }

                var deactivatedUserSubscription = await _UserSubscriptionRepository.DeactivateUserSubscriptionAsync(userSubscriptionId);
                if (deactivatedUserSubscription == null)
                {
                    return ApiResponse<UserSubscription>.Fail("UserSubscription not found or already inactive.", 404);
                }

                return ApiResponse<UserSubscription>.Ok("UserSubscription deactivated successfully.", deactivatedUserSubscription);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<UserSubscription>.Fail($"Invalid UserSubscription ID: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserSubscription>.Fail("An error occurred while deactivating the UserSubscription.", 500, ex);
            }
        }

        public async Task<ApiResponse<UserSubscription>> ActivateUserSubscriptionAsync(Guid userSubscriptionId)
        {
            try
            {
                if (userSubscriptionId == Guid.Empty)
                {
                    throw new ArgumentException("UserSubscription ID cannot be empty.", nameof(userSubscriptionId));
                }

                var activatedUserSubscription = await _UserSubscriptionRepository.ActivateUserSubscriptionAsync(userSubscriptionId);
                if (activatedUserSubscription == null)
                {
                    return ApiResponse<UserSubscription>.Fail("UserSubscription not found or already active.", 404);
                }

                return ApiResponse<UserSubscription>.Ok("UserSubscription activated successfully.", activatedUserSubscription);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<UserSubscription>.Fail($"Invalid UserSubscription ID: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserSubscription>.Fail("An error occurred while activating the UserSubscription.", 500, ex);
            }
        }
       
    }
}