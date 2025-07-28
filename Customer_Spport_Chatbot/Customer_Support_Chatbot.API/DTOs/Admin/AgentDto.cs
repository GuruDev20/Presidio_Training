namespace Customer_Support_Chatbot.DTOs.Admin
{
    public class AgentDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}