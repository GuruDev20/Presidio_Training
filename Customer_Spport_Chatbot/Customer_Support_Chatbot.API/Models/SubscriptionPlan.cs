namespace Customer_Support_Chatbot.Models
{
    public class SubscriptionPlan
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public List<string>? Features { get; set; } 
        public int DurationInDays { get; set; } = 0;
        public int Priority { get; set; } // Higher number means higher priority
        public ICollection<UserSubscription>? UserSubscriptions { get; set; }
    }
}