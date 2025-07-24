namespace Customer_Support_Chatbot.Models
{
    public class UserSubscription
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid PlanId { get; set; }
        public SubscriptionPlan? Plan { get; set; }

        public Guid PaymentId { get; set; }
        public Payment? Payment { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}