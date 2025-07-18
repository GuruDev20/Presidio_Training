namespace Customer_Support_Chatbot.Models.DTOs.Payment
{
    public class PaymentDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public string RazorpayPaymentId { get; set; } = string.Empty;
    }
}