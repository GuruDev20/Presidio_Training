namespace Customer_Support_Chatbot.Models.DTOs.Subscription
{
    public class SubscriptionDto
    {
        public Guid UserId { get; set; }
        public Guid PaymentId { get; set; }
        public string Tier { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}