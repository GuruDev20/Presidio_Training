namespace Customer_Support_Chatbot.DTOs.Chat
{
    public class SendMessageDto
    {
        public Guid TicketId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}