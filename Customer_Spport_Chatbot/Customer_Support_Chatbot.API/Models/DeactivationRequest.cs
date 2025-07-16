namespace Customer_Support_Chatbot.Models
{
    public class DeactivationRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = "Pending"; 
        public User User { get; set; } = null!;
    }
}