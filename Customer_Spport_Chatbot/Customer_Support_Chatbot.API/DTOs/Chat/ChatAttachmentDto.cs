namespace Customer_Support_Chatbot.DTOs.Chat
{
    public class ChatAttachmentDto
    {
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime SendAt { get; set; } = DateTime.UtcNow;
    }
}