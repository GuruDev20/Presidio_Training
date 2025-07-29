namespace Customer_Support_Chatbot.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public Order? Order { get; set; }
        public Guid OrderId { get; set; }
        public UserSubscription? UserSubscription { get; set; }
    }
}
