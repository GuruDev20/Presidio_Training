namespace Customer_Support_Chatbot.Models.DTOs.Order
{
    public class OrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Amount { get; set; } = 0;
        public string ContactNumber { get; set; } = string.Empty;
    }
}