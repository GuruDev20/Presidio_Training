namespace Customer_Support_Chatbot.Models.DTOs.UserSubscription
{
    public class UserSubscriptionDto
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}