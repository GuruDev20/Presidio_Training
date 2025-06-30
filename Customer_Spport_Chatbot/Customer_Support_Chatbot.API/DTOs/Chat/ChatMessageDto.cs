namespace Customer_Support_Chatbot.DTOs.Chat
{
    public class ChatMessageDto
    {
        public string Content { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

}