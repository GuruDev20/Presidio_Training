namespace Customer_Support_Chatbot.DTOs.Admin
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? AgentUsername { get; set; } = string.Empty;
    }
}