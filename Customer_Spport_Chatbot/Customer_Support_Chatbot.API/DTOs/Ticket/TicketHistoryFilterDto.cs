namespace Customer_Support_Chatbot.DTOs.Ticket
{
    public class TicketHistoryFilterDto
    {
        public Guid UserOrAgentId { get; set; }
        public string Role { get; set; } = string.Empty; // "User" or "Agent"
        public string? SearchKeyword { get; set; } = string.Empty;
        public string? TimeRange { get; set; } = string.Empty; // e.g., "Last 24 hours", "Last 7 days", "Last 30 days"
    }
}