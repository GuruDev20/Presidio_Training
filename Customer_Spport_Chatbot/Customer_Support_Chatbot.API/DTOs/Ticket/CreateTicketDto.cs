namespace Customer_Support_Chatbot.DTOs.Ticket
{
    public class CreateTicketDto
    {
        public Guid UserId { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}