using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Customer_Support_Chatbot.Repositories
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly AppDbContext _context;
        public SubscriptionPlanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync()
        {
            return await _context.SubscriptionPlans.ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetSubscriptionPlanByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Subscription Plan ID cannot be empty.", nameof(id));
            }
            return await _context.SubscriptionPlans.FindAsync(id);
        }

        public async Task<SubscriptionPlan> CreateSubscriptionPlan(SubscriptionPlanDto plan)
        {
            var subscriptionPlan = new SubscriptionPlan
            {
                Name = plan.Name,
                Price = plan.Price,
                Description = plan.Description,
                Features = plan.Features,
                DurationInDays = plan.DurationInDays
            };
            _context.SubscriptionPlans.Add(subscriptionPlan);
            await _context.SaveChangesAsync();
            return subscriptionPlan;
        }

        public async Task<SubscriptionPlan?> UpdateSubscriptionPlanAsync(Guid planId, SubscriptionPlanDto planDto)
        {
            var existing = await _context.SubscriptionPlans.FindAsync(planId);
            if (existing == null) return null;

            existing.Name = planDto.Name;
            existing.Price = planDto.Price;
            existing.Description = planDto.Description;
            existing.Features = planDto.Features;
            existing.DurationInDays = planDto.DurationInDays;

            _context.Entry(existing).CurrentValues.SetValues(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteSubscriptionPlanAsync(Guid id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null) return false;
            _context.SubscriptionPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}