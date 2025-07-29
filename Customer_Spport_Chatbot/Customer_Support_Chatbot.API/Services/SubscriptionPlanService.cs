using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Customer_Support_Chatbot.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        public SubscriptionPlanService(ISubscriptionPlanRepository subscriptionPlanRepository)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
        }

        public async Task<ApiResponse<IEnumerable<SubscriptionPlan>>> GetAllSubscriptionPlansAsync()
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetAllSubscriptionPlansAsync();
                return ApiResponse<IEnumerable<SubscriptionPlan>>.Ok("Subscription plans retrieved successfully.", plans);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<SubscriptionPlan>>.Fail("An error occurred while retrieving subscription plans.", 500, ex.Message);
            }
        }

        public async Task<ApiResponse<SubscriptionPlan>> GetSubscriptionPlanByIdAsync(Guid id)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetSubscriptionPlanByIdAsync(id);
                if (plan == null)
                {
                    return ApiResponse<SubscriptionPlan>.Fail($"Subscription plan with ID {id} not found.", 404);
                }
                return ApiResponse<SubscriptionPlan>.Ok("Subscription plan retrieved successfully.", plan);
            }
            catch (Exception ex)
            {
                return ApiResponse<SubscriptionPlan>.Fail($"An error occurred while retrieving the subscription plan with ID {id}.", 500, ex.Message);
            }
        }

        public async Task<ApiResponse<SubscriptionPlan>> CreateSubscriptionPlanAsync(SubscriptionPlanDto planDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(planDto.Name))
                    throw new ArgumentException("Plan name cannot be empty.");
                if (planDto.Price < 0)
                    throw new ArgumentException("Plan price cannot be negative.");
                if (planDto.DurationInDays <= 0)
                    throw new ArgumentException("Duration must be greater than zero.");
                if (string.IsNullOrWhiteSpace(planDto.Description))
                    throw new ArgumentException("Description cannot be empty.");

                var addedPlan = await _subscriptionPlanRepository.CreateSubscriptionPlan(planDto);
                if (addedPlan == null)
                {
                    throw new Exception("Failed to add new subscription plan.");
                }
                return ApiResponse<SubscriptionPlan>.Ok("Subscription plan created successfully.", addedPlan);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<SubscriptionPlan>.Fail($"Invalid subscription plan data: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<SubscriptionPlan>.Fail("An error occurred while adding the subscription plan.", 500, ex.Message);
            }
        }

        public async Task<ApiResponse<SubscriptionPlan>> UpdateSubscriptionPlanAsync(Guid planId, SubscriptionPlanDto planDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(planDto.Name))
                    throw new ArgumentException("Plan name cannot be empty.");
                if (planDto.Price < 0)
                    throw new ArgumentException("Plan price cannot be negative.");
                if (planDto.DurationInDays <= 0)
                    throw new ArgumentException("Duration must be greater than zero.");
                if (string.IsNullOrWhiteSpace(planDto.Description))
                    throw new ArgumentException("Description cannot be empty.");

                var updatedPlan = await _subscriptionPlanRepository.UpdateSubscriptionPlanAsync(planId, planDto);
                if (updatedPlan == null)
                {
                    return ApiResponse<SubscriptionPlan>.Fail($"Subscription plan with ID {planId} not found.", 404);
                }
                return ApiResponse<SubscriptionPlan>.Ok("Subscription plan updated successfully.", updatedPlan);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<SubscriptionPlan>.Fail($"Invalid subscription plan data: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<SubscriptionPlan>.Fail("An error occurred while updating the subscription plan.", 500, ex);
            }
        }

        public async Task<ApiResponse<bool>> DeleteSubscriptionPlanAsync(Guid id)
        {
            try
            {
                var deleted = await _subscriptionPlanRepository.DeleteSubscriptionPlanAsync(id);
                if (!deleted)
                {
                    return ApiResponse<bool>.Fail($"Subscription plan with ID {id} not found.", 404);
                }
                return ApiResponse<bool>.Ok("Subscription plan deleted successfully.", true);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail("An error occurred while deleting the subscription plan.", 500, ex.Message);
            }
        }
    }
}