namespace Customer_Support_Chatbot.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public Payment? Payment { get; set; }
        public Guid? PaymentId { get; set; }
    }
}