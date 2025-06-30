namespace Customer_Support_Chatbot.DTOs.Chat
{
    public class ChatSessionDto
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public List<ChatMessageDto> Messages { get; set; } = new();
        public List<ChatAttachmentDto> Attachments { get; set; } = new();
    }
}