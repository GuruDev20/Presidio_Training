namespace Customer_Support_Chatbot.API.DTOs.Chat
{
    public class LeaveChatDto
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public bool IsAgent { get; set; }
    }
}