namespace Customer_Support_Chatbot.Models.DTOs.UserSubscription
{
    public class SubscriptionPlanDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public ICollection<string>? Features { get; set; } 
        public int DurationInDays { get; set; } = 0;
    }
}