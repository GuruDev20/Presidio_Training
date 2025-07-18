namespace Customer_Support_Chatbot.DTOs.Chat
{
    public class UnifiedMessageDto
    {
        public string Sender { get; set; } = string.Empty; // "user" or "agent"
        public string? Text { get; set; }
        public string? FileUrl { get; set; }
        public bool IsImage { get; set; }
        public string Timestamp { get; set; } = string.Empty;
    }
}