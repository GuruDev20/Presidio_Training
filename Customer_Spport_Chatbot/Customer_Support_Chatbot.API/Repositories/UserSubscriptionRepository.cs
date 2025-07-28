using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.UserSubscription;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Repositories
{
    public class UserSubscriptionRepository : Repository<UserSubscription>, IUserSubscriptionRepository
    {
        private new readonly AppDbContext _context;
        public UserSubscriptionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserSubscription> CreateUserSubscriptionAsync(UserSubscriptionDto userSubscriptionDto)
        {
            var userSubscription = new UserSubscription
            {
                UserId = userSubscriptionDto.UserId,
                PlanId = userSubscriptionDto.PlanId,
                PaymentId = userSubscriptionDto.PaymentId,
                StartDate = userSubscriptionDto.StartDate,
                EndDate = userSubscriptionDto.EndDate,
                Status = "Active"
            };

            _context.UserSubscriptions.Add(userSubscription);
            await _context.SaveChangesAsync();
            return userSubscription;
        }

        public async Task<UserSubscription?> GetUserSubscriptionByIdAsync(Guid id){
            return await _context.UserSubscriptions.FindAsync(id);
        }

        public async Task<IEnumerable<UserSubscription>> GetAllUserSubscriptionsByUserIdAsync(Guid userId){
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            return await _context.UserSubscriptions
                .Where(us => us.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserSubscription>> GetAllUserSubscriptionsAsync(){
            return await _context.UserSubscriptions
                .Include(us => us.Plan)
                .ToListAsync();
        }

        public async Task<UserSubscription?> DeactivateUserSubscriptionAsync(Guid userSubscriptionId)
        {
            var userSubscription = await _context.UserSubscriptions.FindAsync(userSubscriptionId);
            if (userSubscription == null)
            {
                return null;
            }

            userSubscription.Status = "Inactive";
            await _context.SaveChangesAsync();
            return userSubscription;
        }

        public async Task<UserSubscription?> ActivateUserSubscriptionAsync(Guid userSubscriptionId)
        {
            var userSubscription = await _context.UserSubscriptions.FindAsync(userSubscriptionId);
            if (userSubscription == null)
            {
                return null;
            }

            userSubscription.Status = "Active";
            await _context.SaveChangesAsync();
            return userSubscription;
        }

    }
}