namespace Customer_Support_Chatbot.DTOs.Chat
{
    public class MessageDto
    {
        public Guid Id{ get; set; }
        public Guid TicketId{ get; set; }
        public Guid SenderId{ get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}