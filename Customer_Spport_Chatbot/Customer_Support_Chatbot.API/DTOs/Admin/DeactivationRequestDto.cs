namespace Customer_Support_Chatbot.DTOs.Admin
{
    public class DeactivationRequestDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
    }
}